using DocsWASM.Client.Pages;
using DocsWASM.Server.Controllers.Document;
using DocsWASM.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using static DocsWASM.Shared.DocumentModels;
using static DocsWASM.Shared.SearchModels;
using DocsWASM.Shared.Serializer;

namespace DocsWASM.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SearchController : ControllerBase
	{
		private readonly ILogger<SearchController> _logger;
		public AppDb Db { get; }
		public SearchController(ILogger<SearchController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		[Route("search")]
		public async Task<IActionResult> Search(string q)
		{
			Dictionary<uint, SearchResult> Results = new();
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @$"
SELECT
	SUBSTRING(paragraphs, GREATEST(1, LOCATE(@search, paragraphs) - 60), 120) AS snippet,
	pages.id as docId,
	pages.pageNo,
	pages.documentId,
	documents.name,
	documents.description,
	documents.subjectId,
	subjects.name  as subjectTypeName,
	ownerUserId, #8
	login.username,
	documents.docType,
	doctypes.name as docTypeName,
	documents.yearGroup,
	documents.school,
	documents.chapterId,
	chapterName,
	documents.createdDate,
	(SELECT GROUP_CONCAT(id SEPARATOR ',') FROM pages WHERE documentId = documents.id) AS pageIds,
	documents.imgPreview,
	approved
FROM
	pages
	JOIN documents ON pages.documentId = documents.id
	INNER JOIN chapters ON documents.chapterId = chapters.id
	INNER JOIN login ON documents.ownerUserId = login.id
	INNER JOIN subjects ON documents.subjectId = subjects.id
	INNER JOIN doctypes ON documents.docType = doctypes.id
WHERE
	paragraphs LIKE CONCAT('%', @search, '%');
";
			cmd.Parameters.AddWithValue("@search", q);

			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
				{
					if (!Results.ContainsKey((uint)reader[3]))
					{
						Results[(uint)reader[3]] = new()
						{
							documentHeader = new()
							{
								DocumentId = (uint)reader[3],
								DocumentName = (string)reader[4],
								Description = reader[5] == System.DBNull.Value ? null : (string)reader[5],
								SubjectType = (uint)reader[6],
								SubjectTypeName = (string)reader[7],
								OwnerUserId = (uint)reader[8],
								OwnerUserName = (string)reader[9],
								DocType = (byte)reader[10],
								DocTypeName = (string)reader[11],
								YearGroup = (string)reader[12],
								SchoolName = (string)reader[13],
								ChapterId = (uint)reader[14],
								ChapterName = (string)reader[15],
								CreatedDate = (DateTime)reader[16],
								Pages = ((string)reader[17]).Split(',').Select(x => uint.Parse(x)),
								ImgPreview = (byte[])reader[18],
								Approved = (byte)reader[19]
							},
							pageMatchs = new()
							{
								new()
								{
									match = (string)reader[0],
									page = (uint)reader[2]
								}
							}
						};
					}
					else
						Results[(uint)reader[3]].pageMatchs.Add(new()
						{
							match = (string)reader[0],
							page = (uint)reader[2]
						});
				}
			var toSend = Results.Select(x => x.Value).ToList();

			return File(SearchResultListSerializer.Serialize(toSend), "application/octet-stream");
		}
	}
}
