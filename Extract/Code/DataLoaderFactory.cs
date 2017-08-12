using Extract;
using Extract;
using Microsoft.SqlServer.Management.Smo;

namespace Extract
{
	public static class DataLoaderFactory
	{

		public static IDataLoader CreateDataLoader(DataType type, string database) {

			switch (DataConfig.DatabaseType) {

				case DataType.Sql:
					Server context = DatabaseContextFactory.CreateSqlDatabaseContext(database);
					IDataLoader dataLoader = SqlDataLoaderFactory.CreateDataLoader(type, context);
					return dataLoader;

				//case Configurations.DatabaseType.MySql:
				//	MySqlLoaderFactory mySqlLoaderFactory = new MySqlDataLoaderFactory();
				//	MySqlConnection context = CreateMySqlDatabaseContext(database);
				//	return loaderFactory.CreateDataLoader(DataType, context);


				default:
					throw new UnsupportedEngineTypeException("database");
			}
		}
	}
}