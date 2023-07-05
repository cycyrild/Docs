using DocsWASM.Shared;
using DocsWASM.Shared.Serializer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using static DocsWASM.Shared.DocumentModels;

namespace DocsWASM.Server.Controllers.Document
{
    [Route("api/[controller]")]
	[ApiController]
	public class PreviewController : ControllerBase
	{
		private readonly ILogger<PreviewController> _logger;

		public PreviewController(ILogger<PreviewController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		public AppDb Db { get; }

		[Route("lastDocuments/all")]
		public async Task<IActionResult> LastDocuments(int limit)
		{
			await Db.Connection.OpenAsync();

			List<DocumentHeader> documentHeaders = new();
			MySqlCommand cmd;
			cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @"
			select
			documents.id,
			documents.name,
			documents.description,
			documents.subjectId,
			documents.ownerUserId,
			login.userName,
			documents.imgPreview,
			documents.docType,
			documents.yearGroup,
			documents.school,
			documents.chapterId,
			documents.createdDate,
			documents.approved
			from documents
			inner join subjects on subjects.id = documents.subjectId
			inner join login on documents.ownerUserId = login.id
			ORDER BY documents.createdDate DESC
			limit @limit";
			cmd.Parameters.AddWithValue("@limit", limit);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					documentHeaders.Add(new()
					{
						DocumentId = (uint)reader[0],
						DocumentName = (string)reader[1],
						Description = reader[2] != System.DBNull.Value ? (string)reader[2] : "",
						SubjectType = (uint)reader[3],
						OwnerUserId = (uint)reader[4],
						OwnerUserName = (string)reader[5],
						ImgPreview = (byte[])reader[6],
						DocType = (byte)reader[7],
						YearGroup = (string)reader[8],
						SchoolName = (string)reader[9],
						ChapterId = (uint)reader[10],
						CreatedDate= (DateTime)reader[11],
						Approved = (byte)reader[12],
						
					});

			Response.Headers["Content-Encoding"] = "br";

			return File(await Helpers.Compression.CompressBytesAsync(DocumentHeaderListSerializer.Serialize(documentHeaders)), "application/octet-stream");
		}

	}
}
