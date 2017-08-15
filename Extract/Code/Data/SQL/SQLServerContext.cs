using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Extract
{
	public class SQLServerContext
	{

		public static Server Server { get { return server; } }

		public string DatabaseName { get { return database; } }
		public Database Database { get { return Server.Databases[database]; } }
		public TableCollection Tables { get { return Database.Tables; } }
		public DatabaseCollection Databases { get { return server.Databases; } }

		private readonly static SQLConfig config = DataConfig.SQLConfig;
		private readonly static Server server = new Server();

		private string database;
		private string connectionString;


		public SQLServerContext(string database) {
			if (string.IsNullOrWhiteSpace(database)) throw new ArgumentNullException("database");
			ChangeDatabase(database);
		}

		public void Refresh() {
			server.Refresh();
			Databases.Refresh();
			Tables.Refresh();
		}


		public void ChangeDatabase(string database) {
			Server.Databases.Refresh();
			this.database = database;
			this.connectionString = string.Format(config.connectionString, config.DataSource, database, config.UserId, config.Password, config.ConnectionTimeout);
		}


		public bool DatabaseExists(string database) {
			return Server.Databases.Contains(database);
		}

		public void ExecuteReader(string commandText, Action<SqlDataReader> action) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				using (SqlCommand command = new SqlCommand(commandText, connection)) {
					connection.Open();
					SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
					action(reader);
				}
			}
		}


		public void ExecuteNonQuery(string commandText) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				using (SqlCommand command = new SqlCommand(commandText, connection)) {
					connection.Open();
					command.ExecuteNonQuery();
				}
			}
		}


		public DataSet SelectRows(string commandText) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				using (SqlDataAdapter adapter = new SqlDataAdapter(commandText, connection)) {
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);
					return dataSet;
				}
			}
		}


		public void Update(string commandText, DataSet dataSet) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				using (SqlDataAdapter adapter = new SqlDataAdapter(commandText, connection)) {
					using (SqlCommandBuilder builder = new SqlCommandBuilder(adapter)) {
						adapter.Update(dataSet);
					}
				}
			}
		}


		public void ExecuteBulkCopy(Action<SqlBulkCopy> action) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection)) {
					connection.Open();
					action(bulkCopy);
				}
			}
		}
	}
}