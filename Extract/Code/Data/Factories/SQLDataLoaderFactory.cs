using System;

namespace Extract
{
	public static class SQLDataLoaderFactory
	{

		public static IDataLoader CreateDataLoader(DataType type, SQLServerContext context) {

			switch (type) {
				case DataType.BAK:
					return new SQLBAKDataLoader(context);

				case DataType.CSV:
					return new SQLCSVDataLoader(context);

				case DataType.XML:
					return new SQLXMLDataLoader(context);

				default:
					throw new ArgumentException("DataType " + type + " not supported");
			}
		}
	}
}