using DocsWASM.Client.Pages;
using DocsWASM.Server.Controllers.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DocsWASM.Shared.Views.SchoolModel;

namespace DocsWASM.Server.Controllers.Fetch
{
	[Route("api/[controller]")]
	[ApiController]
	public class BrowseFolderController : ControllerBase
	{
		private readonly ILogger<BrowseFolderController> _logger;

		public BrowseFolderController(ILogger<BrowseFolderController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		public AppDb Db { get; }

		
	}
}
