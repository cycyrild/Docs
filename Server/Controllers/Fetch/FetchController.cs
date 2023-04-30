using DocsWASM.Client.Pages;
using DocsWASM.Server;
using DocsWASM.Server.Controllers.Document;
using DocsWASM.Shared.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Runtime.CompilerServices;
using static DocsWASM.Shared.TypeOfUsersModels;
using static DocsWASM.Shared.Views.SchoolModel;

namespace DocsWASM.Controllers.Fetch
{
	[Route("api/[controller]")]
	[ApiController]
	public class FetchController : ControllerBase
	{
		private readonly ILogger<FetchController> _logger;

		public FetchController(ILogger<FetchController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}


		public AppDb Db { get; }

        [Route("typeofusers")]
		public async Task<Dictionary<byte, TypeOfUsers>> GetTypeOfUsers()
		{
			var userTypeByteNameimgHTML = new Dictionary<byte, TypeOfUsers> ();
            await Db.Connection.OpenAsync();
            MySqlCommand cmd;
            cmd = Db.Connection.CreateCommand();
			cmd.CommandText = "select userType, name, imgSRC from usertypes";
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
					userTypeByteNameimgHTML[(byte)reader[0]] = new TypeOfUsers() { Name = (string)reader[1], imgSRC = (string)reader[2] };

			return userTypeByteNameimgHTML;
        }


	}
}
