using DocsWASM.Client.Pages;
using DocsWASM.Server.Controllers.Document;
using DocsWASM.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using static DocsWASM.Shared.DocumentModels;
using static DocsWASM.Shared.SearchModels;
using DocsWASM.Shared.Serializer;
using Fizzler;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mysqlx.Crud;

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
		/*		[Route("search")]
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
*/

		[Route("search")]
		public async Task<IActionResult> Search(string q, int? page, int? limit)
		{
			Dictionary<uint, SearchResult> Results = new();
			await Db.Connection.OpenAsync();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			var cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @$"
SELECT
	pages.paragraphs,
	pages.id as docId,
	pages.pageNo,
	pages.documentId,
	documents.name,
	documents.description,
	documents.subjectId,
	subjects.name  as subjectTypeName,
	documents.ownerUserId,
	login.username,
	documents.docType,
	doctypes.name as docTypeName,
	documents.yearGroup,
	documents.school,
	documents.chapterId,
	chapters.chapterName,
	documents.createdDate,
	(SELECT GROUP_CONCAT(id SEPARATOR ',') FROM pages WHERE documentId = documents.id) AS pageIds,
	documents.imgPreview,
	documents.approved,
	CASE
		WHEN documents.name LIKE CONCAT('%',@search, '%') THEN 1
		ELSE 0
	END as name_match,
	CASE
		WHEN pages.paragraphs LIKE CONCAT('%',@search, '%') THEN 1
		ELSE 0
	END as paragraphs_match
FROM
	pages
	JOIN documents ON pages.documentId = documents.id
	INNER JOIN chapters ON documents.chapterId = chapters.id
	INNER JOIN login ON documents.ownerUserId = login.id
	INNER JOIN subjects ON documents.subjectId = subjects.id
	INNER JOIN doctypes ON documents.docType = doctypes.id
WHERE
	pages.paragraphs LIKE CONCAT('%', @search, '%') 
	OR documents.name LIKE CONCAT('%', @search, '%')
{(page == null || limit == null ? "" : "LIMIT @limit OFFSET @offset")};";

			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);

			cmd.Parameters.AddWithValue("@search", q);

			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
				{
					if ((int)reader[20]==1) //name match
					{
						if (!Results.ContainsKey((uint)reader[3]))
						{
							Results[(uint)reader[3]] = new SearchResult()
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
								pageMatchs = null,
							};
						}
						continue;
					}

					var fullText = Text.CleanString((string)reader[0]);
					var wordIndex = 0;
					while ((wordIndex = fullText.IndexOf(q, wordIndex, StringComparison.OrdinalIgnoreCase)) != -1)
					{
						var startIndex = Math.Max(0, wordIndex - 150);
						var endIndex = Math.Min(fullText.Length, wordIndex + 150);
						var snippet = fullText.Substring(startIndex, endIndex - startIndex);

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
									{ (uint)reader[2], new List<string>(){ snippet } }
								}
							};
						}
						else
						{
							if (Results[(uint)reader[3]].pageMatchs.ContainsKey((uint)reader[2]))
								Results[(uint)reader[3]].pageMatchs[(uint)reader[2]].Add(snippet);
							else
								Results[(uint)reader[3]].pageMatchs[(uint)reader[2]] = new List<string>() { snippet };

						}


						wordIndex += q.Length;
					}

				}
			var toSend = Results.Select(x => x.Value).ToList();

			Response.Headers["Content-Encoding"] = "br";

			return File(await Helpers.Compression.CompressBytesAsync(SearchResultListSerializer.Serialize(toSend)), "application/octet-stream");

		}
	}
}
