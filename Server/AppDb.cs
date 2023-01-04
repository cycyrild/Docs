using System;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DocsWASM.Server
{
	public class AppDb : IDisposable
	{
		public MySqlConnection Connection { get; }

		public AppDb(string connectionString)
		{
			Connection = new MySqlConnection(connectionString);
		}

		public void Dispose() => Connection.Dispose();
	}
}
