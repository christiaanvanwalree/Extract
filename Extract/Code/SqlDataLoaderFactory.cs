using Microsoft.SqlServer.Management.Smo;
using System;

namespace Extract
{
	public static class SqlDataLoaderFactory
	{

		public static IDataLoader CreateDataLoader(DataType type, Server context) {

			switch (type) {
				case DataType.Sql:
					return new SqlToSqlDataLoader(context);

				case DataType.Csv:
					return new CsvToSqlDataLoader(context);

				case DataType.Xml:
					throw new NotImplementedException("XML");

				default:
					throw new ArgumentException("Type " + type + " does not exist");
			}
		}
	}
}