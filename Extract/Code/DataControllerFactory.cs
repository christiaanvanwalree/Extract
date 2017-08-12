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

				case DataType.Sql:

					Server context = DatabaseContextFactory.CreateSqlDatabaseContext(database);
					return new SqlDataController(context);

				//case Configurations.DatabaseType.MySql:
				//	MySqlLoaderFactory mySqlLoaderFactory = new MySqlDataLoaderFactory();
				//	MySqlConnection context = CreateMySqlDatabaseContext(database);
				//	return loaderFactory.CreateDataLoader(DataType, context);


				default:
					throw new UnsupportedEngineTypeException("database type");
			}
		}
	}
}