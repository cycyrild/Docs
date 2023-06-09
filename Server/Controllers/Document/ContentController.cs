using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocsWASM.Shared.Views;
using System.Collections.Generic;
using DocsWASM.Shared;
using DocsWASM.Shared.Helpers;
using DocsWASM.Client.Pages;
using MySql.Data.MySqlClient;
using static DocsWASM.Shared.PathModels;
using Path = DocsWASM.Shared.PathModels.Path;
using DocsWASM.Shared.Serializer;

namespace DocsWASM.Server.Controllers.Document
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContentController : ControllerBase
	{
		private readonly ILogger<ContentController> _logger;

		public ContentController(ILogger<ContentController> logger, AppDb db)
		{
			_logger = logger;
			Db = db;
		}

		public AppDb Db { get; }

		[Route("Schools")]
		public async Task<List<SchoolModel>> GetSchools(int? page, int? limit)
		{
			var list = new List<SchoolModel>();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @$"select schools.name, schools.description, svgLogo, cast(count(schools.name) as unsigned) as documentCount from schools
                                inner join documents on documents.school = schools.name
                                {(page == null || limit == null ? "" : "LIMIT @limit OFFSET @offset")}";
			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					if(reader[0]!=System.DBNull.Value)
						list.Add(new()
						{
							Name = (string)reader[0],
							Description = reader[1] != System.DBNull.Value ? (string)reader[1] : null,
							SvgLogo = reader[2] != System.DBNull.Value ? (string)reader[2] : null,
							DocumentCount = (ulong)reader[3]
						});
			return list;
		}

		[Route("YearGroups")]
		public async Task<List<YearGroupModel>> GetYearGroups(string? school, int? page, int? limit)
		{
			var list = new List<YearGroupModel>();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();
			cmd.CommandText = $@"select school, yearGroup, description from yeargroups {(school != null ? "where school = @school" : "")}
                                {(page == null || limit == null ? "" : "LIMIT @limit OFFSET @offset")}";
			if (school != null) cmd.Parameters.AddWithValue("@school", school);
			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					list.Add(new()
					{
						School = (string)reader[0],
						Name = (string)reader[1],
						Description = reader[2] != System.DBNull.Value ? (string)reader[2] : null,
					});
			return list;
		}

		[Route("Subjects")]
		public async Task<List<SubjectModel>> GetSubjects(string? school, string? yearGroup, int? page, int? limit)
		{
			var conditions = new List<string>();
			var list = new List<SubjectModel>();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();

			if (school != null)
			{
				conditions.Add("documents.school = @school");
				cmd.Parameters.AddWithValue("@school", school);
			}
			if (yearGroup != null)
			{
				conditions.Add("yearGroup = @yearGroup");
				cmd.Parameters.AddWithValue("@yearGroup", yearGroup);
			}

			cmd.CommandText = $@"SELECT  subjects.id,
								subjects.name,
								svgLogo,
								cast(count(a.subjectId) as UNSIGNED) as count 
								FROM subjects
								LEFT JOIN (
										select subjectId, school, yearGroup from documents {(conditions.Count > 0 ? $"where {String.Join(" and ", conditions)}" : "")}
									) AS a
								on subjects.id = a.subjectId
								GROUP BY subjects.id, subjects.name, subjects.svgLogo
								order by count desc
								{(page == null || limit == null ? "" : "LIMIT @limit OFFSET @offset")};";

			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);

			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
						list.Add(new()
						{
							Id = (uint)reader[0],
							Name = (string)reader[1],
							SvgLogo = reader[2] != System.DBNull.Value ? (string)reader[2] : null,
							Count = (ulong)reader[3]
						});
			return list;
		}

		[Route("Chapters")]
		public async Task<List<ChapterModel>> GetChapters(string? school, string? yearGroup, uint? subjectId, int? page, int? limit)
		{
			var conditions = new List<string>();
			var list = new List<ChapterModel>();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();

			if (school != null)
			{
				conditions.Add("documents.school = @school");
				cmd.Parameters.AddWithValue("@school", school);
			}
			if (yearGroup != null)
			{
				conditions.Add("yearGroup = @yearGroup");
				cmd.Parameters.AddWithValue("@yearGroup", yearGroup);
			}
			if (subjectId != null)
				cmd.Parameters.AddWithValue("@subjectId", subjectId);

			cmd.CommandText = @$"SELECT chapters.id, chapterName, subjects.name, chapters.subjectId, chapterDescription, CAST(COUNT(documents.id) AS UNSIGNED INT) AS count
								FROM chapters
								LEFT JOIN documents ON documents.chapterId = chapters.id {(conditions.Count > 0 ? $"and {String.Join(" and ", conditions)}" : "")}
								LEFT JOIN subjects ON chapters.subjectId = subjects.id
								{(subjectId != null ? "where chapters.subjectId = @subjectId" : "")}
								GROUP BY chapters.id, subjects.name
								order by count desc
                                {(page == null || limit == null ? "" : "LIMIT @limit OFFSET @offset")}";

			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);

			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
						list.Add(new()
						{
							Id = (uint)reader[0],
							Name = (string)reader[1],
							Subject = (string)reader[2],
							SubjectId = (uint)reader[3],
							Description = reader[4] != System.DBNull.Value ? (string)reader[4] : null,
							Count = (ulong)reader[5]
						});
			return list;
		}

		[Route("DocTypes")]
		public async Task<List<DocumentTypeModel>> GetDocumentTypes(string? school, string? yearGroup, uint? subjectId, uint? chapterId, int? page, int? limit)
		{
			var conditions = new List<string>();
			var list = new List<DocumentTypeModel>();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();

			if (school != null)
			{
				conditions.Add("documents.school = @school");
				cmd.Parameters.AddWithValue("@school", school);
			}
			if (yearGroup != null)
			{
				conditions.Add("yearGroup = @yearGroup");
				cmd.Parameters.AddWithValue("@yearGroup", yearGroup);
			}
			if (subjectId != null)
			{
				conditions.Add("documents.subjectId = @subjectId");
				cmd.Parameters.AddWithValue("@subjectId", subjectId);
			}
			if (chapterId != null)
			{
				conditions.Add("chapterId = @chapterId");
				cmd.Parameters.AddWithValue("@chapterId", chapterId);
			}

			cmd.CommandText = @$"SELECT doctypes.id AS docTypeId, doctypes.name, COALESCE(COUNT(documents.docType), 0) AS count
								FROM doctypes
								LEFT JOIN documents ON doctypes.id = documents.docType
								LEFT JOIN chapters ON documents.chapterId = chapters.id
								{(conditions.Count > 0 ? $"where {String.Join(" and ", conditions)}" : "")}
								GROUP BY doctypes.id;

                                {(page == null || limit == null ? "" : "LIMIT @limit OFFSET @offset")}";

			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);

			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
						list.Add(new()
						{
							Id = (Byte)reader[0],
							Name = (string)reader[1],
							Count = (ulong)((long)reader[2])
						});
			return list;
		}

		[Route("Documents")]
		public async Task<IActionResult> GetDocument(string? school, string? yearGroup, string? subjectId, string? chapterId, string? docTypeId, int? page, int? limit, int? approved)
		{
			var conditions = new List<string>();
			var documents = new List<DocumentModels.DocumentHeader>();
			int offset = (page == null || limit == null ? -1 : limit.Value * page.Value);
			await Db.Connection.OpenAsync();
			var cmd = Db.Connection.CreateCommand();

			if (school != null)
			{
				conditions.Add("documents.school = @school");
				cmd.Parameters.AddWithValue("@school", school);
			}
			if (yearGroup != null)
			{
				conditions.Add("documents.yearGroup = @yearGroup");
				cmd.Parameters.AddWithValue("@yearGroup", yearGroup);
			}
			if (subjectId != null)
			{
				conditions.Add("documents.subjectId = @subjectId");
				cmd.Parameters.AddWithValue("@subjectId", subjectId);
			}
			if (docTypeId != null)
			{
				conditions.Add("documents.docType = @docTypeId");
				cmd.Parameters.AddWithValue("@docTypeId", docTypeId);
			}
			if (chapterId != null)
			{
				conditions.Add("documents.chapterId = @chapterId");
				cmd.Parameters.AddWithValue("@chapterId", chapterId);
			}
			if(approved!=null)
			{
				conditions.Add("documents.approved = @approved");
				cmd.Parameters.AddWithValue("@approved", approved);
			}

			cmd.CommandText = @$"SELECT	documents.id,
									documents.name,
									documents.description,
									documents.subjectId,
									subjects.name  as subjectTypeName,
									ownerUserId,
									login.username,
									documents.docType,
									doctypes.name as docTypeName,
									documents.yearGroup,
									documents.school,
									documents.chapterId,
									chapterName,
									documents.createdDate,
									GROUP_CONCAT(pages.id SEPARATOR ','),
									documents.imgPreview,
									approved
								FROM documents
								JOIN pages ON documents.id = pages.documentId
								inner join chapters on documents.chapterId = chapters.id
								inner join login on documents.ownerUserId = login.id
								inner join subjects on documents.subjectId = subjects.id
								inner join doctypes on documents.docType = doctypes.id
                                {(conditions.Count > 0 ? $"where {String.Join(" and ", conditions)}" : "")}
								GROUP BY documents.id
                                {(page == null || limit == null ? "" : "LIMIT @limit OFFSET @offset")}";

			cmd.Parameters.AddWithValue("@limit", limit);
			cmd.Parameters.AddWithValue("@offset", offset);

			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					documents.Add(new()
					{
						DocumentId = (uint)reader[0],
						DocumentName = (string)reader[1],
						Description = reader[2] != System.DBNull.Value ? (string)reader[2] : null,
						SubjectType = (uint)reader[3],
						SubjectTypeName = (string)reader[4],
						OwnerUserId = (uint)reader[5],
						OwnerUserName = (string)reader[6],
						DocType = (Byte)reader[7],
						DocTypeName = (string)reader[8],
						YearGroup = (string)reader[9],
						SchoolName = (string)reader[10],
						ChapterId = (uint)reader[11],
						ChapterName = (string)reader[12],
						CreatedDate = (DateTime)reader[13],
						Pages = ((string)reader[14]).Split(',').Select(x => uint.Parse(x)),
						ImgPreview = (byte[])reader[15],
						Approved = (byte)reader[16]
					});

			return File(DocumentHeaderListSerializer.Serialize(documents), "application/octet-stream");
		}


		public async Task<List<Path>> GetPath(MySqlConnection conn, string? school, string? yearGroup, string? subjectId, string? chapterId, string? docTypeId)
		{
			MySqlCommand cmd;
			var list = new List<Path>();
			list.Add(new Path(school, paths.schools));
			list.Add(new Path(yearGroup, paths.yearGroups));

			cmd = conn.CreateCommand();
			cmd.CommandText = "select name from subjects where id = @id limit 1";
			cmd.Parameters.AddWithValue("@id", subjectId);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					list.Add(new PathModels.Path((string)reader[0], paths.subjects));

			cmd = conn.CreateCommand();
			cmd.CommandText = "select chapterName from chapters where id = @id limit 1";
			cmd.Parameters.AddWithValue("@id", chapterId);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					list.Add(new PathModels.Path((string)reader[0], paths.chapters));

			return list;
		}

	}
}

