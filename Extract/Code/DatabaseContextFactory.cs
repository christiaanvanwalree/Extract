using Extract;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;

namespace Extract
{
	public static class DatabaseContextFactory
	{
		public static Server CreateSqlDatabaseContext(string database = null) {
			SqlServerConfig config = DataConfig.SqlServerConfig;
			SqlConnection sqlConnection = new SqlConnection(string.Format(DataConfig.SqlServerConfig.SqlConnectionUrl, config.DataSource, (database == null) ? config.InitialCatalog : database, config.UserId, config.Password, config.ConnectionTimeout));
			ServerConnection serverConnection = new ServerConnection(sqlConnection);
			return new Server(serverConnection);
		}

		public static MySqlConnection CreateMySqlDatabaseContext(string database) {
			if (string.IsNullOrWhiteSpace(database)) throw new ArgumentNullException("database");

			MySqlServerConfig config = DataConfig.MySqlServerConfig;
			MySqlConnection connection = new MySqlConnection(string.Format(DataConfig.MySqlServerConfig.MysqlConnectionUrl, config.Server, config.Port, config.UserId, config.Password, database));
			return connection;
		}
	}
}