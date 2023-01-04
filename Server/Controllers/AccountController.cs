using DocsWASM.Server.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using static DocsWASM.Shared.AccountModels;
using static DocsWASM.Server.Helper;
namespace DocsWASM.Account
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private static string connectionStrg = DBConnectionString();
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet("my")]
        public async Task<User> GetCurrentUserInfo()
        {
            var userId = uint.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            using (MySqlConnection conn = new MySqlConnection(connectionStrg))
            {
                await conn.OpenAsync();

                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT
                                        id,
                                        userName,
                                        firstName,
                                        lastName,
                                        email,
                                        createdDate,
                                        modifiedDate,
                                        lastLogin,
                                        bio,
                                        fullNamePrivacy,
                                        createdIp,
                                        lastIp,
                                        userType
                                    FROM login
                                    WHERE id = @id
                                    LIMIT 0, 1";
                cmd.Parameters.AddWithValue("@id", userId);
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        return new()
                        {
                            Id = (uint)reader[0],
                            UserName = (string)reader[1],
                            FirstName = (string)reader[2],
                            LastName = (string)reader[3],
                            Email = (string)reader[4],
                            CreatedDate = (DateTime)reader[5],
                            ModifiedDate = reader[6] != System.DBNull.Value ? (DateTime)reader[6] : null,
                            LastLogin = reader[7] != System.DBNull.Value ? (DateTime)reader[7] : null,
                            Bio = (string)reader[8],
                            FullNamePrivacy = Convert.ToBoolean((ulong)reader[9]),
                            CreatedIp = reader[10] != System.DBNull.Value ? (string)reader[10] : null,
                            LastIp = reader[11] != System.DBNull.Value ? (string)reader[11] : null,
                            TypeOfUser = (UserType)((Byte)reader[12]),
                        };
                    }
            }
            return new();
        }
    }
}
