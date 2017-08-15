using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;

namespace Extract
{
	public class SQLCSVDataLoader : IDataLoader
	{

		private readonly SQLDatabaseController controller;

		public SQLCSVDataLoader(SQLDatabaseController controller) {
			if (controller == null) throw new ArgumentNullException("controller");

			this.controller = controller;
		}

		public string Export(string database) {
			if (!SQLDatabaseController.DatabaseExists(database)) throw new InvalidOperationException("database does not exist");

			string folderName = Guid.NewGuid().ToString();
			string zipName = folderName + DataConfig.zipExt;
			string absoluteFolderPath = Path.Combine(DataConfig.baseDir, DataConfig.exportDir, folderName);
			string absoluteZipPath = Path.Combine(DataConfig.baseDir, DataConfig.exportDir, zipName);

			Directory.CreateDirectory(absoluteFolderPath);
			TableCollection tables = controller.Tables;

			for (int i = 0; i < tables.Count; i++) {

				string tableName = tables[i].Name;

				StreamWriter writer = new StreamWriter(Path.Combine(absoluteFolderPath, tableName + DataConfig.csvExt));
				string selectQuery = SQLQueryGenerator.GetSelectQuery(tableName);
				controller.ExecuteReader(selectQuery, (reader) => CSVReaderWriter.CreateCsvFile(reader, writer));
			}

			ZipFile.CreateFromDirectory(absoluteFolderPath, absoluteZipPath);
			Directory.Delete(absoluteFolderPath, true);
			return Path.Combine(DataConfig.exportDir, zipName);
		}

		
		public void Load(DataFile file) {

			if (!SQLDatabaseController.DatabaseExists(file.database)) {
				string createDatabaseQuery = SQLQueryGenerator.GetCreateDatabaseQuery(file.database);
				controller.ExecuteNonQuery(createDatabaseQuery);
				controller.ChangeDatabase(file.database);
			}

			DataTable csvData = CSVReaderWriter.ReadCsvDataTable(file.path);
			controller.ExecuteBulkCopy((bulkCopy) => CreateDatabaseTable(file, csvData, bulkCopy));
		}


		private void CreateDatabaseTable(DataFile file, DataTable data, SqlBulkCopy bulkCopy) {

			DataColumnCollection columns = data.Columns;
			string[] columnNames = new string[columns.Count];

			for (int i = 0; i < columns.Count; i++) {
				string columnName = columnNames[i] = columns[i].ColumnName;
				bulkCopy.ColumnMappings.Add(columnName, columnName);
			}

			string createTableQuery = SQLQueryGenerator.GetCreateTableQuery(file.fileName, columnNames);
			controller.ExecuteNonQuery(createTableQuery);
			controller.Refresh();

			bulkCopy.DestinationTableName = SQLQueryGenerator.EncloseInBrackets(file.fileName);
			bulkCopy.WriteToServer(data);
		}
	}
}