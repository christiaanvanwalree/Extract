using System;

namespace Extract
{
	public static class SQLDataLoaderFactory
	{

		public static IDataLoader CreateDataLoader(DataType type, SQLServerContext context) {

			switch (type) {
				case DataType.SQL:
					return new SQLBAKDataLoader(context);

				case DataType.CSV:
					return new SQLCSVLoader(context);

				case DataType.XML:
					throw new NotImplementedException("XML");

				default:
					throw new ArgumentException("Type " + type + " does not exist");
			}
		}
	}
}