using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.IO;

namespace Extract
{
	public static class CSVReaderWriter
	{

		public static DataTable ReadCsvDataTable(string path) {

			DataTable csvData = new DataTable();
			TextFieldParser csvReader = new TextFieldParser(path);

			csvReader.SetDelimiters(new string[] { "," });
			csvReader.HasFieldsEnclosedInQuotes = true;

			string[] colFields = csvReader.ReadFields();

			for (int i = 0; i < colFields.Length; i++) {
				DataColumn datecolumn = new DataColumn(colFields[i]);
				datecolumn.AllowDBNull = true;
				csvData.Columns.Add(datecolumn);
			}

			while (!csvReader.EndOfData) {

				string[] fieldData = csvReader.ReadFields();

				for (int i = 0; i < fieldData.Length; i++) {
					if (string.IsNullOrEmpty(fieldData[i])) {
						fieldData[i] = null;
					}
				}

				csvData.Rows.Add(fieldData);
			}

			return csvData;
		}

		public static void CreateCsvFile(IDataReader reader, StreamWriter writer) {
			string Delimiter = "\"";
			string Separator = ",";

			for (int i = 0; i < reader.FieldCount; i++) {
				if (i > 0) {
					writer.Write(Separator);
				}
				writer.Write(Delimiter + reader.GetName(i) + Delimiter);
			}

			writer.WriteLine(string.Empty);

			while (reader.Read()) {
				for (int i = 0; i < reader.FieldCount; i++) {
					if (i > 0) {
						writer.Write(Separator);
					}
					writer.Write(Delimiter + reader.GetValue(i).ToString().Replace('"', '\'') + Delimiter);
				}
				writer.WriteLine(string.Empty);
			}

			writer.Flush();
			writer.Close();
			reader.Close();
		}
	}
}