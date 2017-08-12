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
					return DataType.Csv;
				case xml:
					return DataType.Xml;
				case sql:
					return DataType.Sql;
				case excel:
					return DataType.Excel;
				case mysql:
					return DataType.MySql;
				default:
					throw new NotImplementedException("type");
			}
		}

		public static string Convert(DataType type) {
			switch (type) {
				case DataType.Csv:
					return csv;
				case DataType.Xml:
					return xml;
				case DataType.Sql:
					return sql;
				case DataType.Excel:
					return excel;
				case DataType.MySql:
					return mysql;
				default:
					throw new NotImplementedException("type");
			}
		}
	}
}