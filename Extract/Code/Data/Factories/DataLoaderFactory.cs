using Microsoft.SqlServer.Management.Smo;

namespace Extract
{
	public static class DataLoaderFactory
	{

		public static IDataLoader CreateDataLoader(DataType type, string database) {

			switch (DataConfig.DatabaseType) {

				case DataType.SQL:
					SQLDatabaseController context = DatabaseContextFactory.CreateSQLDatabaseContext(database);
					IDataLoader dataLoader = SQLDataLoaderFactory.CreateDataLoader(type, context);
					return dataLoader;

				//case Configurations.DatabaseType.MySQL:
				//	MySQLLoaderFactory mySQLLoaderFactory = new MySQLDataLoaderFactory();
				//	MySqlConnection context = CreateMySQLDatabaseContext(database);
				//	return loaderFactory.CreateDataLoader(DataType, context);


				default:
					throw new UnsupportedEngineTypeException("database");
			}
		}
	}
}