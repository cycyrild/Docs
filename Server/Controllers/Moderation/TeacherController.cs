using DocsWASM.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static DocsWASM.Shared.AccountModels;
using static DocsWASM.Server.Helper;

namespace DocsWASM.Server.Controllers.Admin
{
	[ApiController]
	[Route("api/[controller]")]
	public class TeacherController : Controller
	{
		private readonly ILogger<TeacherController> _logger;
		public AppDb Db { get; }

		public TeacherController(ILogger<TeacherController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		private async Task<bool> CheckIfTeacher(uint id)
		{
			var cmd = Db.Connection.CreateCommand();
			cmd.CommandText = "SELECT COUNT(*) > 0 AS has_usertype_3 FROM login WHERE id = @id AND userType = 3;";
			cmd.Parameters.AddWithValue("@id", id);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					return (int)reader[0] == 1;

			return false;
		}


		[HttpGet]
		[Route("dashboard")]
		public async Task<IActionResult> AdminDashboard(int? page, int? limit)
		{
			await Db.Connection.OpenAsync();
			
			if (User != null && uint.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
				&& await CheckIfTeacher(userId))
			{

				return Content("1");

				//SELECT COUNT(*) > 0 AS has_usertype_3 FROM login WHERE id = 13 AND userType = 3;
			}
			return Content("0");
		}
	}
}
