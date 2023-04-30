using DocsWASM.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Tls;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using static DocsWASM.Shared.UploadModels;

namespace DocsWASM.Server.Controllers.Document
{
    [ApiController]
	[Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(ILogger<DocumentsController> logger, AppDb db)
        {
            _logger = logger;
            Db = db;
        }

        public AppDb Db { get; }


        [Route("GetYearsGroups")]
        public async Task<Dictionary<string, List<string>>> GetYearsGroups()
        {
            await Db.Connection.OpenAsync();
			Dictionary<string, List<string>> schoolYearGroups = new();
            MySqlCommand cmd = Db.Connection.CreateCommand();
            cmd.CommandText = "select school, yearGroup from yeargroups";
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    if (!schoolYearGroups.ContainsKey((string)reader[0]))
                        schoolYearGroups[(string)reader[0]] = new List<string>() { (string)reader[1] };
                    else
					    schoolYearGroups[(string)reader[0]].Add((string)reader[1]);
				}
			return schoolYearGroups;
        }

        [Route("GetDocTypes")]
        public async Task<Dictionary<byte, string>> GetDocTypes()
        {
			await Db.Connection.OpenAsync();
			Dictionary<byte, string> strings = new();
            MySqlCommand cmd = Db.Connection.CreateCommand();
            cmd.CommandText = "select id, name from doctypes";
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    strings.Add((byte)reader[0], (string)reader[1]);
            return strings;
        }

        [Route("GetSchools")]
        public async Task<string[]> GetSchools()
        {
			await Db.Connection.OpenAsync();
			List<string> strings = new();
            MySqlCommand cmd = Db.Connection.CreateCommand();
            cmd.CommandText = "select name from schools";
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    strings.Add((string)reader[0]);
            return strings.ToArray();
        }

        /*[Route("GetChapters")]
        public async Task<Dictionary<uint, string>> GetChapters()
        {
			await Db.Connection.OpenAsync();
			Dictionary<uint, string> strings = new();
            MySqlCommand cmd = Db.Connection.CreateCommand();
            cmd.CommandText = "select id, chapterName from chapters";
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    strings.Add((uint)reader[0], (string)reader[1]);
            return strings;
        }

        [Route("GetSubjects")]
        public async Task<Dictionary<uint, string>> GetSubjects()
        {
			await Db.Connection.OpenAsync();
			Dictionary<uint, string> strings = new();
            MySqlCommand cmd = Db.Connection.CreateCommand();
            cmd.CommandText = "select id, name from subjects";
            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                    strings.Add((uint)reader[0], (string)reader[1]);
            return strings;
        }*/

        [Route("GetSubjectsChapters")]
        public async Task<SubjectsChapters> GetSubjectsChapters()
        {
            await Db.Connection.OpenAsync();
            var subjects = new Dictionary<uint, string>();
            var chapters = new Dictionary<uint, List<SubjectsChapters.chapter>>();


            MySqlCommand cmd;
            cmd = Db.Connection.CreateCommand();
			cmd.CommandText = "select id, name from subjects";
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					subjects.Add((uint)reader[0], (string)reader[1]);

            foreach(var subject in subjects)
            {
				cmd = Db.Connection.CreateCommand();
				cmd.CommandText = "select id, chapterName from chapters where subjectId = @subjectId";
                cmd.Parameters.AddWithValue("@subjectId", subject.Key);
                using (var reader = await cmd.ExecuteReaderAsync())
                    while (await reader.ReadAsync())
                    {
                        if (chapters.ContainsKey(subject.Key))
                            chapters[subject.Key].Add(new SubjectsChapters.chapter() { id = (uint)reader[0], name = (string)reader[1] });
                        else
                            chapters[subject.Key] = new() { new SubjectsChapters.chapter() { id = (uint)reader[0], name = (string)reader[1] } };

					}
			}
            return new() { subjects = subjects, chapters = chapters };
		}

	}
}
