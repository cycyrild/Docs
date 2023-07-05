using DocsWASM.Server.Annotations;
using DocsWASM.Server.Controllers.Members;
using DocsWASM.Shared.Annotations;
using DocsWASM.Shared.Serializer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using static DocsWASM.Shared.AccountModels;

namespace DocsWASM.Server.Controllers.Document.Annotation
{
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class AnnotationsController : ControllerBase
	{
		private readonly ILogger<AnnotationsController> _logger;

		public AnnotationsController(ILogger<AnnotationsController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		public AppDb Db { get; }

		[HttpPost("send")]
		public async Task<IActionResult> AnnotationsPost(uint docId)
		{
			using (var ms = new MemoryStream(1024 * 20000))
			{
				await Request.Body.CopyToAsync(ms);
				ms.Position = 0;
				var byteArray = ms.ToArray();
				var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
				await Db.Connection.OpenAsync();

				var comparer = new AnnotationEqualityComparer();
				var annotations = AnnotationListSerializer.Deserialize(byteArray);
				var currAnotations = await FetchAnnotations.GetAnnotations(docId, Db.Connection);

				var removed = currAnotations.Except(annotations, comparer).Where(a => a.UserId == userId).ToList();
				var added = annotations.Except(currAnotations, comparer).Where(a => a.UserId == userId).ToList();

				foreach (var annotation in removed)
				{
					var cmd = Db.Connection.CreateCommand();
					cmd.CommandText = "DELETE FROM annotations WHERE id = @id";
					cmd.Parameters.AddWithValue("@id", annotation.Id);
					await cmd.ExecuteNonQueryAsync();
				}


				foreach (var annotation in added)
				{
					var cmd = Db.Connection.CreateCommand();
					cmd.CommandText = "INSERT INTO annotations (pageId, userId, x, y, text) " +
										   "VALUES (@pageId, @userId, @x, @y, @text)";
					cmd.Parameters.AddWithValue("@pageId", annotation.PageId);
					cmd.Parameters.AddWithValue("@userId", userId);
					cmd.Parameters.AddWithValue("@x", annotation.Point.X);
					cmd.Parameters.AddWithValue("@y", annotation.Point.Y);
					cmd.Parameters.AddWithValue("@text", annotation.Text);
					await cmd.ExecuteNonQueryAsync();
				}

			}
			return Ok();
		}
	}
}
