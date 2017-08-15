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

		public static SQLServerContext CreateSQLDatabaseContext(string database = null) {
			database = (database == null) ? DataConfig.SQLConfig.InitialCatalog : database;
			SQLServerContext context = new SQLServerContext(database);
			return context;
		}

		public static MySqlConnection CreateMySQLDatabaseContext(string database) {
			if (string.IsNullOrWhiteSpace(database)) throw new ArgumentNullException("database");

			MySQLConfig config = DataConfig.MySQLConfig;
			MySqlConnection connection = new MySqlConnection(string.Format(DataConfig.MySQLConfig.MysqlConnectionUrl, config.Server, config.Port, config.UserId, config.Password, database));
			return connection;
		}
	}
}