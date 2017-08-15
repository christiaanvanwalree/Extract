using MySql.Data.MySqlClient;
using System;

namespace Extract.MySQL
{
	public class MySQLDataLoader : IDataLoader
	{

		private readonly MySqlConnection connection;

		public MySQLDataLoader(MySqlConnection connection) {
			if (connection == null) throw new ArgumentNullException("connection");

			this.connection = connection;
		}

		public string Export(string database) {
			return string.Empty;
		}

		public void Load(DataFile file) {
			MySqlCommand command = new MySqlCommand();
			MySqlBackup backup = new MySqlBackup(command);

			command.Connection = connection;
			connection.Open();
			backup.ImportFromFile(file.path);
			connection.Close();
		}

		public void Unload(DataFile file) {
			MySqlCommand command = new MySqlCommand("DROP DATABASE IF EXISTS @database;", connection);
			command.Parameters.Add(new MySqlParameter("@database", file.database));

			connection.Open();
			command.ExecuteNonQuery();
			connection.Close();
		}

		public bool DatabaseExists(string database) {
			throw new NotImplementedException("mysql database exists");
		}
	}
}