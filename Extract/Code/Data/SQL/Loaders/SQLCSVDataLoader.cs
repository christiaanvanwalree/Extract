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
		private readonly SQLServerContext context;


		public SQLCSVDataLoader(SQLServerContext context) {
			if (context == null) throw new ArgumentNullException("context");
			this.context = context;
		}

		public DataFile Export(string database) {
			if (!context.DatabaseExists(database)) throw new InvalidOperationException("database does not exist");

			string folderName = Guid.NewGuid().ToString();
			string zipName = folderName + DataConfig.zipExt;
			string absoluteFolderPath = Path.Combine(DataConfig.baseDir, DataConfig.exportDir, folderName);
			string absoluteZipPath = Path.Combine(DataConfig.baseDir, DataConfig.exportDir, zipName);

			Directory.CreateDirectory(absoluteFolderPath);
			TableCollection tables = context.Tables;

			for (int i = 0; i < tables.Count; i++) {

				string tableName = tables[i].Name;

				StreamWriter writer = new StreamWriter(Path.Combine(absoluteFolderPath, tableName + DataConfig.csvExt));
				string selectQuery = SQLQueryGenerator.GetSelectQuery(tableName);
				context.ExecuteReader(selectQuery, (reader) => CSVReadWrite.CreateCsvFile(reader, writer));
			}

			ZipFile.CreateFromDirectory(absoluteFolderPath, absoluteZipPath);
			Directory.Delete(absoluteFolderPath, true);

			DataFile file = new DataFile(Path.Combine(DataConfig.baseDir, DataConfig.exportDir, zipName), zipName, database);
			return file;
		}

		
		public void Import(DataFile file) {

			string database = file.database;

			if (!context.DatabaseExists(database)) {
				context.CreateDatabase(database);
				context.ChangeDatabase(database);
			}

			DataTable csvData = CSVReadWrite.ReadCsvDataTable(file.path);
			context.ExecuteBulkCopy((bulkCopy) => CreateDatabaseTable(file, csvData, bulkCopy));
		}


		private void CreateDatabaseTable(DataFile file, DataTable data, SqlBulkCopy bulkCopy) {

			DataColumnCollection columns = data.Columns;
			string[] columnNames = new string[columns.Count];

			for (int i = 0; i < columns.Count; i++) {
				string columnName = columnNames[i] = columns[i].ColumnName;
				bulkCopy.ColumnMappings.Add(columnName, columnName);
			}

			string createTableQuery = SQLQueryGenerator.GetCreateTableQuery(file.fileName, columnNames);
			context.ExecuteNonQuery(createTableQuery);
			context.Refresh();

			bulkCopy.DestinationTableName = SQLQueryGenerator.EncloseInBrackets(file.fileName);
			bulkCopy.WriteToServer(data);
		}
	}
}