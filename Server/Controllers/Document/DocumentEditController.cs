using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using static DocsWASM.Shared.UploadModels;
using static DocsWASM.Shared.Helpers.Bson;
using static DocsWASM.Shared.DocumentModels;

namespace DocsWASM.Server.Controllers.Document
{
	[Route("api/[controller]")]
	[ApiController]
    public class DocumentEditController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;

        public DocumentEditController(ILogger<ContentController> logger, AppDb db)
        {
            _logger = logger;
            Db = db;
        }

        public AppDb Db { get; }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task Delete(int id)
        {
            MySqlCommand cmd = Db.Connection.CreateCommand();
            await Db.Connection.OpenAsync();
            cmd.CommandText = "DELETE FROM documents WHERE id = @id LIMIT 1";
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public async Task Edit(int id)
        {
			MySqlCommand cmd = Db.Connection.CreateCommand();
			using (var ms = new MemoryStream(1024 * 100000))
            {
                await Request.Body.CopyToAsync(ms);
                ms.Position = 0;
				var uploadSend = FromBson<DocumentHeader>(ms.ToArray());
				await Db.Connection.OpenAsync();
				cmd.CommandText = "UPDATE documents " +
                    "SET name = @name, " +
                    "description = @description " +
					"where id = @id LIMIT 1";
				cmd.Parameters.AddWithValue("@id", id);
				cmd.Parameters.AddWithValue("@name", uploadSend.DocumentName);
				cmd.Parameters.AddWithValue("@description", uploadSend.Description);
				await cmd.ExecuteNonQueryAsync();
			}
		}
    }
}
