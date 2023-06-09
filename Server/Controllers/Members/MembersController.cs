using DocsWASM.Shared.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static DocsWASM.Shared.AccountModels;

namespace DocsWASM.Server.Controllers.Members
{
	[Route("api/[controller]")]
	[ApiController]
	public class MembersController : ControllerBase
	{
		private readonly ILogger<MembersController> _logger;

		public MembersController(ILogger<MembersController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		public AppDb Db { get; }

		[Route("ranking")]
		public async Task<List<RankingModele>> Ranking(int? page, int? limit)
		{
			var ranking = new List<RankingModele>();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();
			cmd.CommandText = $@"
			SELECT 
				login.id,
				login.username,
				login.userType,
				cast(count(a.id) as UNSIGNED) as count 
			FROM login
			LEFT join
				(
					select ownerUserId, id from documents where approved = 1
				) AS a
			on login.id = a.ownerUserId
			GROUP BY login.id, login.username, login.userType
			order by count desc
			{(page != null && limit != null ? @"limit @limit
			offset @offset" : "")}
			";
			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);


			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					ranking.Add(new()
					{
						UploadCount = (int)((ulong)reader[3]),
						Member = new User()
						{
							Id = (uint)reader[0],
							UserName = (string)reader[1],
							TypeOfUser = (byte)reader[2]
						}
					});


			return ranking;
		}
	}
}
