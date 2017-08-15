using System;

namespace Extract
{
	public static class SQLDataLoaderFactory
	{

		public static IDataLoader CreateDataLoader(DataType type, SQLDatabaseController context) {

			switch (type) {
				case DataType.SQL:
					return new SQLDataLoader(context);

				case DataType.Csv:
					return new SQLCSVDataLoader(context);

				case DataType.Xml:
					throw new NotImplementedException("XML");

				default:
					throw new ArgumentException("Type " + type + " does not exist");
			}
		}
	}
}