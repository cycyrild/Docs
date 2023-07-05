using MySql.Data.MySqlClient;

namespace DocsWASM.Server.Annotations
{
	public class FetchAnnotations
	{
		public static async Task<List<DocsWASM.Shared.Annotations.Annotation>> GetAnnotations(uint docId, MySqlConnection conn)
		{
			var annotations = new List<DocsWASM.Shared.Annotations.Annotation>();
			var cmd = conn.CreateCommand();
			cmd.CommandText = @"
			SELECT pages.id as pageId, annotations.id as annotionId, login.id as userId, annotations.x, annotations.y, annotations.text, login.username, annotations.modifiedDate
			FROM documents
			JOIN pages ON documents.id = documents.id
			JOIN annotations ON pages.id = annotations.pageId
			JOIN login ON login.id = annotations.userId
			WHERE documents.id =  @documentId;";
			cmd.Parameters.AddWithValue("@documentId", docId);
			using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
					annotations.Add(new()
					{
						PageId = (uint)reader[0],
						Id = (uint)reader[1],
						UserId = (uint)reader[2],
						Point = new Shared.Annotations.Point() { X = (double)reader[3], Y = (double)reader[4] },
						Text = (string)reader[5],
						UserName = (string)reader[6],
						ModifiedDate = (DateTime)reader[7],
					});
			return annotations;
		}
	}
}
