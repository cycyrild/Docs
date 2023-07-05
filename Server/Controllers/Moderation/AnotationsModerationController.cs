using DocsWASM.Server.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static DocsWASM.Server.Controllers.Members.PermissionControl;

namespace DocsWASM.Server.Controllers.Moderation
{

	[TeacherOrAdminAuthorize]
	[ApiController]
	[Route("api/[controller]")]
	public class AnotationsModerationController : ControllerBase
	{
		private readonly ILogger<AnotationsModerationController> _logger;
		public AppDb Db { get; }

		public AnotationsModerationController(ILogger<AnotationsModerationController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}


	}
}
