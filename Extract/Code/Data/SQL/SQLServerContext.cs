using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Extract
{
	public class SQLServerContext : IDatabaseContext
	{

		public static Server Server { get { return server; } }
		public string DatabaseName { get { return database; } }
		public Database Database { get { return Server?.Databases[database]; } }
		public TableCollection Tables { get { return Database?.Tables; } }
		public DatabaseCollection Databases { get { return Server?.Databases; } }

		private readonly static SQLConfig config = DataConfig.SQLConfig;
		private readonly static Server server = new Server();

		private string database;
		private string connectionString;


		public SQLServerContext(string database) {
			if (string.IsNullOrWhiteSpace(database)) throw new ArgumentNullException("database");
			this.database = database = (DatabaseExists(database)) ? database : config.InitialCatalog;
			ChangeDatabase(database);
		}

		public void Refresh() {
			Server?.Refresh();
			Databases?.Refresh();
			Tables?.Refresh();
		}


		public void ChangeDatabase(string database) {
			this.database = database;
			this.connectionString = string.Format(config.connectionString, config.DataSource, database, config.UserId, config.Password, config.ConnectionTimeout);
			Refresh();
		}


		public bool DatabaseExists(string database) {
			if (string.IsNullOrWhiteSpace(database)) return false; 
			return Server.Databases.Contains(database);
		}


		public void CreateDatabase(string database) {
			if (DatabaseExists(database)) throw new InvalidOperationException("database already exists");
			string query = SQLQueryGenerator.GetCreateDatabaseQuery(database);
			ExecuteNonQuery(query);
			ChangeDatabase(database);
		}


		public void DropDatabase(string database) {
			if (!DatabaseExists(database)) throw new InvalidOperationException("database does not exists");
			if (this.database.Equals(database)) ChangeDatabase(config.InitialCatalog);
			server.KillDatabase(database);
			Refresh();
		}


		public void ExecuteReader(string commandText, Action<SqlDataReader> action) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				using (SqlCommand command = new SqlCommand(commandText, connection)) {
					connection.Open();
					SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
					action(reader);
				}
			}
			Refresh();
		}


		public void ExecuteNonQuery(string commandText) {
			try {
				using (SqlConnection connection = new SqlConnection(connectionString)) {
					using (SqlCommand command = new SqlCommand(commandText, connection)) {
						connection.Open();
						command.ExecuteNonQuery();
					}
				}
			} catch (SqlException e) {
				throw new InvalidOperationException(e.Message);
			}
			Refresh();
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
			Refresh();
		}
	}
}