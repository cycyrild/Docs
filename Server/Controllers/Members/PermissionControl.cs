using MySql.Data.MySqlClient;

namespace DocsWASM.Server.Controllers.Members
{
    public class PermissionControl
    {
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
