using Extract;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;

namespace Extract
{
	public class CsvToSqlDataLoader : IDataLoader
	{

		private readonly Server server;
		private readonly SqlConnection connection;

		public CsvToSqlDataLoader(Server server) {
			if (server == null) throw new ArgumentNullException("server");

			this.server = server;
			this.connection = server.ConnectionContext.SqlConnectionObject;
		}

		public string Export(string database) {
			string folderName = Guid.NewGuid().ToString();
			string zipName = folderName + DataConfig.zipExtension;
			string relativeFolderPath = Path.Combine(DataConfig.exportFolder, folderName);
			string relativeZipPath = Path.Combine(DataConfig.exportFolder, zipName);
			string absoluteFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFolderPath);
			string absoluteZipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeZipPath);

			Directory.CreateDirectory(absoluteFolderPath);

			List<string> tableNames = new List<string>();
			Dictionary<string, List<string>> tableColumnNames = new Dictionary<string, List<string>>();
			TableCollection tables = server.Databases[database].Tables;

			for (int i = 0; i < tables.Count; i++) {

				Table table = tables[i];
				string tableName = table.Name;
				tableNames.Add(tableName);
				tableColumnNames.Add(tableName, new List<string>());

				for (int j = 0; j < table.Columns.Count; j++) {
					tableColumnNames[tableName].Add(table.Columns[j].Name);
				}

			}

			for (int i = 0; i < tableNames.Count; i++) {

				string tableName = tableNames[i];
				List<string> columnNames = tableColumnNames[tableName];

				SqlCommand command = new SqlCommand("SELECT * FROM [" + tableNames[i] + "];", connection);
				SqlDataReader reader = command.ExecuteReader();

				StreamWriter writer = new StreamWriter(Path.Combine(absoluteFolderPath, tableName + DataConfig.csvExtension));
				CreateCsvFile(reader, writer);

				reader.Close();
				writer.Close();
			}

			ZipFile.CreateFromDirectory(absoluteFolderPath, absoluteZipPath);
			Directory.Delete(absoluteFolderPath, true);

			return relativeZipPath;
		}

		private void CreateCsvFile(SqlDataReader reader, StreamWriter writer) {
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
		}

		public void Load(DataFile file) {

			DataTable csvData = ReadCsvDataTable(file);
			SqlConnection connection = server.ConnectionContext.SqlConnectionObject;
			SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);

			CreateDatabase(file, csvData.Columns, connection, ref bulkCopy);
			bulkCopy.WriteToServer(csvData);
		}

		public void Unload(DataFile file) {
			if (server.Databases.Contains(file.database)) {
				server.KillDatabase(file.database);
			}
		}

		private void CreateDatabase(DataFile file, DataColumnCollection columns, SqlConnection connection, ref SqlBulkCopy bulkCopy) {

			if (!server.Databases.Contains(file.database)) {
				SqlCommand createDatabaseCommand = new SqlCommand("CREATE DATABASE[" + file.database + "]", connection);
				createDatabaseCommand.ExecuteNonQuery();
			}

			bool tableExists = false;

			for (int i = 0; i < columns.Count; i++) {
				string columnName = columns[i].ToString();
				bulkCopy.ColumnMappings.Add(columnName, columnName);

				string sqlTable = (tableExists) ? "ALTER TABLE" : "CREATE TABLE";
				string sqlColumn = (tableExists) ? "ADD [" + columnName + "] nvarchar(MAX)" : "([" + columnName + "] nvarchar(MAX))";
				
				connection.ChangeDatabase(file.database);
				SqlCommand createTableCommand = new SqlCommand(sqlTable + " [" + file.fileName + "] " + sqlColumn, connection);
				createTableCommand.ExecuteNonQuery();

				bulkCopy.DestinationTableName = "[" + file.fileName + "]";

				tableExists = true;
			}
		}

		private DataTable ReadCsvDataTable(DataFile file) {

			DataTable csvData = new DataTable();
			TextFieldParser csvReader = new TextFieldParser(file.path);

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
	}
}