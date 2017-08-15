using Extract;
using Extract;
using Microsoft.SqlServer.Management.Smo;
using System;

namespace Extract
{
	public static class DataControllerFactory
	{

		public static IDataController CreateDataController(string database) {
			if (string.IsNullOrWhiteSpace(database)) throw new ArgumentNullException("database");

			switch (DataConfig.DatabaseType) {

				case DataType.SQL:

					SQLDatabaseController context = DatabaseContextFactory.CreateSQLDatabaseContext(database);
					SQLDataController controller = new SQLDataController(context);
					return controller;

				//case Configurations.DatabaseType.MySQL:
				//	MySQLLoaderFactory mySQLLoaderFactory = new MySQLDataLoaderFactory();
				//	MySqlConnection context = CreateMySQLDatabaseContext(database);
				//	return loaderFactory.CreateDataLoader(DataType, context);


				default:
					throw new UnsupportedEngineTypeException("database type");
			}
		}
	}
}