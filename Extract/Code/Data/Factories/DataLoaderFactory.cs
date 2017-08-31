using Microsoft.SqlServer.Management.Smo;
using System;

namespace Extract
{
	public static class DataLoaderFactory
	{
		public static IDataLoader CreateDataLoader(DataType dataType, string database) {
			if (string.IsNullOrWhiteSpace(database)) throw new ArgumentNullException("database");

			switch (DataConfig.DatabaseType) {

				case DataType.SQL:
					SQLServerContext context = DatabaseContextFactory.CreateSQLDatabaseContext(database);
					IDataLoader loader = SQLDataLoaderFactory.CreateDataLoader(dataType, context);
					return loader;

				default:
					throw new UnsupportedEngineTypeException("database");
			}
		}


		//casting IDatabaseContext is allowed since DatabaseType defines the context that is created.
		public static IDataLoader CreateDataLoader(DataType dataType, IDatabaseContext context) {
			if (context == null) throw new ArgumentNullException("context");

			switch (DataConfig.DatabaseType) {

				case DataType.SQL:
					IDataLoader loader = SQLDataLoaderFactory.CreateDataLoader(dataType, (SQLServerContext)context);
					return loader;

				default:
					throw new UnsupportedEngineTypeException("database");
			}
		}


		//casting IDatabaseContext is allowed since DatabaseType defines the context that is created.
		public static IDataLoader CreateDefaultExporter(string database, IDatabaseContext context) {
			if (string.IsNullOrWhiteSpace(database)) throw new ArgumentNullException("database");
			if (context == null) throw new ArgumentNullException("context");

			switch (DataConfig.DatabaseType) {

				case DataType.SQL:
					IDataLoader exporter = SQLDataLoaderFactory.CreateDataLoader(DataType.CSV, (SQLServerContext)context);
					return exporter;

				default:
					throw new UnsupportedEngineTypeException("database");
			}
		}
	}
}