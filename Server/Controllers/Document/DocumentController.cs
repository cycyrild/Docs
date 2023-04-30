using DocsWASM.Shared;
using DocsWASM.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http.Headers;
using static DocsWASM.Shared.UploadModels;

namespace DocsWASM.Server.Controllers.Document
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		private readonly ILogger<DocumentController> _logger;

		public DocumentController(ILogger<DocumentController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		public AppDb Db { get; }

		[Route("View/{id}")]
		public async Task<IActionResult> GetDocument(uint id)
		{
			await Db.Connection.OpenAsync();

			DocumentModels.Document document = new();
			MySqlCommand cmd;
			cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @"
			SELECT	documents.id,
				documents.name,
				documents.description,
				documents.subjectId,
				subjects.name  as subjectTypeName,
				ownerUserId,
				login.username,
				imgPreview,
				documents.docType,
				doctypes.name as docTypeName,
				documents.yearGroup,
				documents.school,
				documents.chapterId,
				chapterName,
				documents.createdDate,
				GROUP_CONCAT(pages.id SEPARATOR ','),
				approved
			FROM documents
			JOIN pages ON documents.id = pages.documentId
			inner join chapters on documents.chapterId = chapters.id
			inner join login on documents.ownerUserId = login.id
			inner join subjects on documents.subjectId = subjects.id
			inner join doctypes on documents.docType = doctypes.id
			where documentId = @id
			GROUP BY documents.id
			limit 1
			";
			cmd.Parameters.AddWithValue("@id", id);

			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					document.DocumentHeader = new()
					{
						DocumentId = (uint)reader[0],
						DocumentName = (string)reader[1],
						Description = reader[2] != System.DBNull.Value ? (string)reader[2] : null,
						SubjectType = (uint)reader[3],
						SubjectTypeName = (string)reader[4],
						OwnerUserId = (uint)reader[5],
						OwnerUserName = (string)reader[6],
						ImgPreview = (byte[])reader[7],
						DocType = (byte)reader[8],
						DocTypeName = (string)reader[9],
						YearGroup = (string)reader[10],
						SchoolName = (string)reader[11],
						ChapterId = (uint)reader[12],
						ChapterName = (string)reader[13],
						CreatedDate = (DateTime)reader[14],
						Pages = ((string)reader[15]).Split(',').Select(x => uint.Parse(x)),
						Approved = (byte)reader[16],
					};

			if (document.DocumentHeader == null) return NotFound();

			document.Page = new();

			cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @"
			select
				id,
				pageNo,
				documentId,
				paragraphs,
				name,
				yearGroup,
				school,
				chapterId,
				docType,
				docBinType,
				subjectId,
				isCorrection,
				bin,
				placeHolder
			from pages
			where documentId = @id";
			cmd.Parameters.AddWithValue("@id", id);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					document.Page.Add(new()
					{
						Id = (uint)reader[0],
						PageNo = (uint)reader[1],
						DocumentId = (uint)reader[2],
						Paragraphs = (string)reader[3],
						Name = (string)reader[4],
						YearGroup = (string)reader[5],
						School = (string)reader[6],
						ChapterId = (uint)reader[7],
						DocType = (Byte)reader[8],
						DocBinType = (dataBinTypesEnum)((Byte)reader[9]),
						SubjectType = (uint)reader[10],
						IsCorrection= Convert.ToBoolean((UInt64)reader[11]),
						Bin = (byte[])reader[12],
						PlaceHolder = (byte[])reader[13]
						
					});

			Response.Headers["Content-Encoding"] = "br";

			return File(await Helpers.Compression.CompressBytesAsync(Bson.ToBson(document)),  "application/x-brotli", $"{id}.br");
		}

	}
}
