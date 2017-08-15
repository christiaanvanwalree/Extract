using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Extract
{
	public static class DataTypeConverter
	{

		private const string csv = "csv";
		private const string xml = "xml";
		private const string sql = "sql";
		private const string excel = "excel";
		private const string mysql = "mysql";

		public static DataType Convert(string type) {
			switch (type.ToLower()) {
				case csv:
					return DataType.CSV;
				case xml:
					return DataType.XML;
				case sql:
					return DataType.SQL;
				case excel:
					return DataType.Excel;
				case mysql:
					return DataType.MySQL;
				default:
					throw new NotImplementedException("type");
			}
		}

		public static string Convert(DataType type) {
			switch (type) {
				case DataType.CSV:
					return csv;
				case DataType.XML:
					return xml;
				case DataType.SQL:
					return sql;
				case DataType.Excel:
					return excel;
				case DataType.MySQL:
					return mysql;
				default:
					throw new NotImplementedException("type");
			}
		}
	}
}