using DocsWASM.Shared;
using DocsWASM.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

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

		[Route("lastDocuments/all/{limit}")]
		public async Task<IActionResult> LastDocuments(int limit)
		{
			await Db.Connection.OpenAsync();

			DocumentModele.PreviewDocumentHeaders documents = new();
			MySqlCommand cmd;
			cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @"
			select
			id,
			name,
			description,
			subjectId,
			ownerUserId,
			imgPreview,
			docType,
			yearGroup,
			school,
			chapterId
			from documents
			ORDER BY createdDate DESC
			limit @limit";
			cmd.Parameters.AddWithValue("@limit", limit);
			documents.Headers = new();
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					documents.Headers.Add(new()
					{
						DocumentId = (uint)reader[0],
						DocumentName = (string)reader[1],
						Description = reader[2] != System.DBNull.Value ? (string)reader[2] : "",
						SubjectType = (uint)reader[3],
						OwnerUserId = (uint)reader[4],
						ImgPreview = (byte[])reader[5],
						DocType = (Byte)reader[6],
						YearGroup = (string)reader[7],
						SchoolName = (string)reader[8],
						ChapterId = (uint)reader[9],
					});
			return File(Bson.ToBson(documents), "application/octet-stream");
		}

	}
}
