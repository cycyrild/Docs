using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using static DocsWASM.Shared.UploadModels;
using static DocsWASM.Shared.DocumentModels;
using DocsWASM.Shared.Serializer;
using Microsoft.AspNetCore.Authorization;
using static DocsWASM.Shared.AccountModels;
using static DocsWASM.Server.Controllers.Members.PermissionControl;
using System.Security.Claims;

namespace DocsWASM.Server.Controllers.Document
{
	[Route("api/[controller]")]
    [Authorize]
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
        public async Task<IActionResult> Delete(int id)
        {
            await Db.Connection.OpenAsync();

            uint.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

            bool isAdminOrTeacher = await CheckIfTeacherOrAdmin(userId, Db.Connection);

            MySqlCommand cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM documents 
                    WHERE id = @id 
                    AND (ownerUserId = @userId OR @isAdminOrTeacher = TRUE) 
                    LIMIT 1";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@userId", userId);

            cmd.Parameters.AddWithValue("@isAdminOrTeacher", isAdminOrTeacher);

            int affectedRows = await cmd.ExecuteNonQueryAsync();

            if (affectedRows > 0)
                return Ok();
            else
                return Unauthorized();
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            await Db.Connection.OpenAsync();

            MySqlCommand cmd = Db.Connection.CreateCommand();

            uint.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

            bool isAdminOrTeacher = await CheckIfTeacherOrAdmin(userId, Db.Connection);
            int affectedRows = 0;
            using (var ms = new MemoryStream(1024 * 100000))
            {
                await Request.Body.CopyToAsync(ms);
                ms.Position = 0;
				var uploadSend = DocumentHeaderSerializer.Deserialize(ms.ToArray());
                cmd.CommandText = @"UPDATE documents 
                        SET name = @name, 
                        description = @description 
                        WHERE id = @id 
                        AND (ownerUserId = @ownerUserId OR @isAdminOrTeacher = TRUE)
                        LIMIT 1";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", uploadSend.DocumentName);
                cmd.Parameters.AddWithValue("@description", uploadSend.Description);
                cmd.Parameters.AddWithValue("@ownerUserId", userId);
                cmd.Parameters.AddWithValue("@isAdminOrTeacher", isAdminOrTeacher);
                affectedRows = await cmd.ExecuteNonQueryAsync();
            }

            if (affectedRows > 0)
                return Ok();
            else
                return Unauthorized();
		}
    }
}
