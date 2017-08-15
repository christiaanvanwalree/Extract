using Microsoft.SqlServer.Management.Smo;

namespace Extract
{
	public static class DataLoaderFactory
	{

		public static IDataLoader CreateDataLoader(DataType type, string database) {

			switch (DataConfig.DatabaseType) {

				case DataType.SQL:
					SQLServerContext context = DatabaseContextFactory.CreateSQLDatabaseContext(database);
					IDataLoader loader = SQLDataLoaderFactory.CreateDataLoader(type, context);
					return loader;

				default:
					throw new UnsupportedEngineTypeException("database");
			}
		}
	}
}