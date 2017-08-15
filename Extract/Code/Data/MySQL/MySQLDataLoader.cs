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

		public DataFile Export(string database) {
			throw new NotImplementedException("MySQL");
		}

		public void Import(DataFile file) {
			MySqlCommand command = new MySqlCommand();
			MySqlBackup backup = new MySqlBackup(command);

			command.Connection = connection;
			connection.Open();
			backup.ImportFromFile(file.path);
			connection.Close();
		}
	}
}