using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Extract
{
	public static class DataModelFactory
	{

		public static void FillColumns(IDataReader reader, ICollection columns, List<ColumnModel> models) {

			List<string> columnNames = new List<string>();
			Dictionary<string, string> columnTypes = new Dictionary<string, string>();
			Dictionary<string, List<string>> columnValues = new Dictionary<string, List<string>>();

			for (int i = 0; i < columns.Count; i++) {
				columnNames.Add(columns.ToString());
			}

			while (reader.Read()) {
				for (int i = 0; i < columnNames.Count; i++) {

					string column = columnNames[i];

					if (!columnValues.ContainsKey(column)) {
						columnValues.Add(column, new List<string>());
					}

					columnValues[column].Add((!reader.IsDBNull(i)) ? reader.GetValue(i).ToString() : string.Empty);

					if (!columnTypes.ContainsKey(column)) {
						columnTypes.Add(column, reader.GetDataTypeName(i));
					}
				}

			}
			reader.Close();

			for (int i = 0; i < columnNames.Count; i++) {
				string columnName = columnNames[i];
				models.Add(new ColumnModel(columnName, columnTypes[columnName].ToString(), columnValues[columnName]));
			}
		}
	}
}