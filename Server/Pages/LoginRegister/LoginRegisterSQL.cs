using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using static DocsWASM.Pages.LoginRegister.LoginRegisterModels;
using static DocsWASM.Server.Helper;
using DocsWASM.Server;
using DocsWASM.Shared;
using System.Net.Mail;
using System.Net;
using System.Xml.Linq;

namespace DocsWASM.Pages.LoginRegister
{
    public class LoginRegisterSQL
    {
        public LoginRegisterSQL()
        {
            Db.Open();
        }
        MySqlConnection Db = new MySqlConnection(DBConnectionString());

        public async Task<LoginResult> Login(string mail, string password, string ip)
        {
            var loginResult = new LoginResult();
            MySqlCommand cmd = Db.CreateCommand();
            cmd.CommandText = @"SELECT id, email, password FROM login WHERE email = @email AND password = @password LIMIT 0, 1";
            cmd.Parameters.AddWithValue("@email", mail);
            cmd.Parameters.AddWithValue("@password", password);

            int rowCount = 0;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    loginResult.id = (uint)reader[0];
                    loginResult.hash = sha256_hash((string)reader[1] + (string)reader[2]);
                    loginResult.success = true;
                    rowCount++;
                }
            }

            if (rowCount == 0)
                return new() { success = false, errorMessage = "Invalid username or password" };

            cmd = Db.CreateCommand();
            cmd.CommandText = "UPDATE login SET lastIp = @ip, lastLogin = current_timestamp() WHERE (email = @email) AND (password = @password) LIMIT 1";
            cmd.Parameters.AddWithValue("@email", mail.ToLower());
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@ip", ip);
            await cmd.ExecuteNonQueryAsync();

            return loginResult;
        }

        public async Task<RegisterResult?> CheckBeforeSendMail(string email, string firstName, string lastName, bool fullNamePrivacy, string password, string name, string ip, byte userType, string school)
        {
            bool emailAlready = false;
            bool nameAlready = false;
            bool fullNameAlready = false;
            Tuple<string, string>? lastCreatedUserIp = null;

            MySqlCommand cmd;
            cmd = Db.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM
                    (SELECT CASE WHEN EXISTS(SELECT username FROM login WHERE username = @username)
                        THEN 0
                        ELSE 1
                    end
                    FROM login limit 1) as x,
                    (SELECT CASE WHEN EXISTS(SELECT email FROM login WHERE email = @email)
                        THEN 0
                        ELSE 1
                    end
                    FROM login LIMIT 1) as y,
		                (SELECT CASE WHEN EXISTS(SELECT lastName, firstName FROM login WHERE lastName = @lastName AND firstName = @firstName)
	                    THEN 0
	                    ELSE 1
	                end
	                FROM login LIMIT 1) as z;
            ";
            cmd.Parameters.AddWithValue("@email", email.ToLower());
            cmd.Parameters.AddWithValue("@username", name);
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    if ((int)reader.GetValue(0) == 0)
                        nameAlready = true;
                    if ((int)reader.GetValue(1) == 0)
                        emailAlready = true;
                    if ((int)reader.GetValue(2) == 0)
                        fullNameAlready = true;
                }
            if (emailAlready && nameAlready)
                return new() { success = false, message = "A user with the same email address and name already exists" };
            if (emailAlready)
                return new() { success = false, message = "A user with the same email address already exists" };
            if (nameAlready)
                return new() { success = false, message = "A user with the same username already exists" };
            if (fullNameAlready)
                return new() { success = false, message = "A user with the same full name already exists" };

            cmd = Db.CreateCommand();
            cmd.CommandText = @"
            SELECT createdIp, email
            FROM login
            where (UNIX_TIMESTAMP(current_timestamp()) - UNIX_TIMESTAMP(createdDate))/60 < 5
            and createdIp = @ip LIMIT 0, 1;
            ";
            cmd.Parameters.AddWithValue("@ip", ip);
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    lastCreatedUserIp = new Tuple<string, string>(((string)reader.GetValue(0)), ((string)reader.GetValue(1)));

            if (lastCreatedUserIp != null)
                return new() { success = false, message = $"An account has already been created less than 5 minutes ago\n(IP:{lastCreatedUserIp.Item1} - User: {lastCreatedUserIp.Item2})" };

            return null;
        }

        public async Task<RegisterResult> RegisterUser(string email, string firstName, string lastName, bool fullNamePrivacy, string password, string name, string ip, byte userType, string school)
        {
            ulong? userID = null;
            bool emailAlready = false;
            bool nameAlready = false;
            bool fullNameAlready = false;
            Tuple<string, string>? lastCreatedUserIp = null;
            MySqlCommand cmd;

			cmd = Db.CreateCommand();
            cmd.CommandText = @"
                SELECT * FROM
                    (SELECT CASE WHEN EXISTS(SELECT username FROM login WHERE username = @username)
                        THEN 0
                        ELSE 1
                    end
                    FROM login limit 1) as x,
                    (SELECT CASE WHEN EXISTS(SELECT email FROM login WHERE email = @email)
                        THEN 0
                        ELSE 1
                    end
                    FROM login LIMIT 1) as y,
		                (SELECT CASE WHEN EXISTS(SELECT lastName, firstName FROM login WHERE lastName = @lastName AND firstName = @firstName)
	                    THEN 0
	                    ELSE 1
	                end
	                FROM login LIMIT 1) as z;
            ";
            cmd.Parameters.AddWithValue("@email", email.ToLower());
            cmd.Parameters.AddWithValue("@username", name);
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    if ((int)reader.GetValue(0) == 0)
                        nameAlready = true;
                    if ((int)reader.GetValue(1) == 0)
                        emailAlready = true;
                    if ((int)reader.GetValue(2) == 0)
                        fullNameAlready = true;
                }
            if (emailAlready && nameAlready)
                return new() { success = false, message = "A user with the same email address and name already exists" };
            if (emailAlready)
                return new() { success = false, message = "A user with the same email address already exists" };
            if (nameAlready)
                return new() { success = false, message = "A user with the same username already exists" };
            if (fullNameAlready)
                return new() { success = false, message = "A user with the same full name already exists" };

            cmd = Db.CreateCommand();
            cmd.CommandText = @"
            SELECT createdIp, email
            FROM login
            where (UNIX_TIMESTAMP(current_timestamp()) - UNIX_TIMESTAMP(createdDate))/60 < 5
            and createdIp = @ip LIMIT 0, 1;
            ";
            cmd.Parameters.AddWithValue("@ip", ip);
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    lastCreatedUserIp = new Tuple<string, string>(((string)reader.GetValue(0)), ((string)reader.GetValue(1)));

            if (lastCreatedUserIp != null)
                return new() { success = false, message = $"An account has already been created less than 5 minutes ago\n(IP:{lastCreatedUserIp.Item1} - User: {lastCreatedUserIp.Item2})" };

            cmd = Db.CreateCommand();
            cmd.CommandText = @"insert into login (email, password, username, createdIp, firstName, lastName, fullNamePrivacy, userType, schoolName) values
                                (@email, @password, @username, @createdIp, @firstName, @lastName, @fullNamePrivacy, @userType, @schoolName);
                                SELECT LAST_INSERT_ID()";
            cmd.Parameters.AddWithValue("@email", email.ToLower());
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@username", name);
            cmd.Parameters.AddWithValue("@createdIp", ip);
            cmd.Parameters.AddWithValue("@firstName", firstName);
            cmd.Parameters.AddWithValue("@lastName", lastName);
            cmd.Parameters.AddWithValue("@fullNamePrivacy", fullNamePrivacy);
            cmd.Parameters.AddWithValue("@userType", userType);
            cmd.Parameters.AddWithValue("@schoolName", school);

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    userID = (ulong)reader.GetValue(0);

            if (userID == null)
                return new() { success = false, message = "Unknown internal error" };
            
            return new() { success = true };
        }

        public async Task<IEnumerable<string>> GetSchools()
        {
            var schools = new List<string>();
            var cmd = Db.CreateCommand();
            cmd.CommandText = @"SELECT name FROM schools";
            using(var reader = await cmd.ExecuteReaderAsync())
                while(await reader.ReadAsync())
                    schools.Add((string)reader.GetValue(0));
            return schools;
        }

        public async Task<bool> CheckSession(ulong id, string hash)
        {
			MySqlCommand cmd = Db.CreateCommand();
                cmd.CommandText = @"SELECT email, password FROM login WHERE ID = @id LIMIT 0, 1";
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                        return sha256_hash((string)reader.GetValue(0) + (string)reader.GetValue(1)) == hash;
                return false;
        }

        public async Task<Dictionary<byte, (string, string)>> GetUserTypes()
        {
            var userTypes = new Dictionary<byte, (string, string)>();
            MySqlCommand cmd = Db.CreateCommand();
            cmd.CommandText = "select * from docas.usertypes";
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    userTypes[(byte)reader[0]] = ((string)reader[1], (string)reader[2]);

            return userTypes;
        }

        public async Task<bool> VerifyPermissionCorrectEmail(string email, byte userType, string schoolName)
        {
            string? extensionDomainMail = email.Split("@").ElementAtOrDefault(1);

            if (extensionDomainMail == null)
                return false;

			MySqlCommand cmd = Db.CreateCommand();
            cmd.CommandText =
				@"
SELECT 
  CASE 
    WHEN EXISTS (select * from validmails where extensionDomainMail = @a1 and userType = @a2 and schoolName = @a3 ) THEN true
    ELSE false
  END AS res;
";
            cmd.Parameters.AddWithValue("@a1", extensionDomainMail);
            cmd.Parameters.AddWithValue("@a2", userType);
            cmd.Parameters.AddWithValue("@a3", schoolName);


			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					return (int)reader[0] == 1;

			return false;
		}

	}
}