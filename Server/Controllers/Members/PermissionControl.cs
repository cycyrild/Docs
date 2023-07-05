using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;

namespace DocsWASM.Server.Controllers.Members
{
    public class PermissionControl
    {
		public class TeacherOrAdminAuthorizeAttribute : Attribute, IAsyncActionFilter
		{
			public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
			{
				// Extract the user ID from the User property of HttpContext
				var userIdString = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (userIdString == null || !uint.TryParse(userIdString, out var userId))
				{
					// If we can't get the user ID, return a 403 status code
					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
					return;
				}

				// Extract the database context from the HttpContext (assuming you're using dependency injection)
				var Db = (AppDb)context.HttpContext.RequestServices.GetService(typeof(AppDb));
				await Db.Connection.OpenAsync();
				// Call your CheckIfTeacherOrAdmin method
				var isAuthorized = await CheckIfTeacherOrAdmin(userId, Db.Connection);

				if (!isAuthorized)
				{
					// If the user isn't authorized, return a 403 status code
					context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
					return;
				}

				// If the user is authorized, continue with the next action
				await next();
			}


		}
		public static async Task<bool> CheckIfTeacherOrAdmin(uint id, MySqlConnection Db)
		{
			var cmd = Db.CreateCommand();
			cmd.CommandText = "SELECT COUNT(*) > 0 AS has_usertype_3 FROM login WHERE id = @id AND (userType = 3 OR userType = 1) LIMIT 1;";
			cmd.Parameters.AddWithValue("@id", id);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
				{
					return (int)reader[0] == 1;
				}

			return false;
		}
	}
}
