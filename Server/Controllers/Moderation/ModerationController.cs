using DocsWASM.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static DocsWASM.Shared.AccountModels;
using static DocsWASM.Server.Helper;
using System.Text.Json;
using static DocsWASM.Shared.Moderation.Moderation;
using Org.BouncyCastle.Asn1.Mozilla;
using DocsWASM.Client.Pages;
using DocsWASM.Shared;
using DocsWASM.Shared.Serializer;
using static DocsWASM.Server.Controllers.Members.PermissionControl;

namespace DocsWASM.Server.Controllers.Admin
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class ModerationController : Controller
	{
		private readonly ILogger<ModerationController> _logger;
		public AppDb Db { get; }

		public ModerationController(ILogger<ModerationController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}



        [HttpPost]
		[Route("moderate")]
		public async Task<IActionResult> Moderation([FromForm] ActionType approve, [FromForm]uint id)
		{
            await Db.Connection.OpenAsync();
            if (User != null && uint.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
			&& await CheckIfTeacherOrAdmin(userId, Db.Connection))
			{
				var cmd = Db.Connection.CreateCommand();
				cmd.CommandText = "UPDATE documents SET approved = @approved WHERE id = @id LIMIT 1;";
				cmd.Parameters.AddWithValue("@id", id);
				if (approve == ActionType.approve)
					cmd.Parameters.AddWithValue("@approved", 1);
				else if (approve == ActionType.unapprove)
					cmd.Parameters.AddWithValue("@approved", 0);
                await cmd.ExecuteNonQueryAsync();

                return Ok();
			}
            return Unauthorized();
		}


		[HttpGet]
		[Route("users")]
		public async Task<IActionResult> Users(int? page, int? limit)
		{
			await Db.Connection.OpenAsync();
			var users = new List<User>();	
			if (User != null && uint.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
				&& await CheckIfTeacherOrAdmin(userId, Db.Connection))
			{
				int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);

				var cmd = Db.Connection.CreateCommand();
				cmd.CommandText = "select username, createdDate, lastIp, lastLogin, firstName, lastName, userType, schoolName " +
				$"from login order by lastLogin {(page != null && limit != null ? "limit @limit offset @offset" : "")};";
				cmd.Parameters.AddWithValue("@limit", limit);
				cmd.Parameters.AddWithValue("@offset", offset);

				using (var reader = await cmd.ExecuteReaderAsync())
					while (await reader.ReadAsync())
					{

						users.Add(new()
						{
							UserName = (string)reader[0],
							CreatedDate = (DateTime)reader[1],
							LastIp = (string)reader[2],
							LastLogin = (DateTime)reader[3],
							FirstName = (string)reader[4],
							LastName = (string)reader[5],
							TypeOfUser = (byte)reader[6],
							SchoolName = (string)reader[7]
						});
					}

				string jsonString = JsonSerializer.Serialize(users);

				return Content(jsonString, "application/json");
			}
			return Unauthorized();
		}
	}
}
