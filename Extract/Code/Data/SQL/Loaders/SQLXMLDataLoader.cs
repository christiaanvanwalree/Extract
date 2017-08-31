using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.IO;

namespace Extract
{
	public class SQLXMLDataLoader : IDataLoader
	{
		private SQLServerContext context;


		public SQLXMLDataLoader(SQLServerContext context) {
			if (context == null) throw new ArgumentNullException("context");
			this.context = context;
		}

		public DataFile Export(string database) {

			string fileName = database + DataConfig.xmlExt;
			string relativePath = Path.Combine(DataConfig.exportDir, fileName);
			string absolutePath = Path.Combine(DataConfig.baseDir, relativePath);

			string query = string.Empty;
			TableCollection tables = context.Tables;

			for (int i = 0; i < tables.Count; i++) {
				if (i > 0) {
					query += ";";
				}
				query += SQLQueryGenerator.GetSelectQuery(tables[i].Name);
			}

			DataSet dataSet = context.SelectRows(query);
			dataSet.DataSetName = database;

			for (int i = 0; i < dataSet.Tables.Count; i++) {
				dataSet.Tables[i].TableName = tables[i].Name;
			}

			DataFile file = new DataFile(absolutePath, fileName, database);
			dataSet.WriteXml(file.path);

			return file;
		}

		public void Import(DataFile file) {
			if (!context.DatabaseExists(file.database)) {
				context.CreateDatabase(file.database);
			}

			DataSet dataSet = new DataSet(file.database);
			dataSet.ReadXml(file.path);
			DataTableCollection tables = dataSet.Tables;

			for (int i = 0; i < tables.Count; i++) {

				DataTable table = tables[i];
				string[] columnNames = new string[table.Columns.Count];

				for (int j = 0; j < table.Columns.Count; j++) {
					columnNames[j] = table.Columns[j].ColumnName;
				}

				string query = SQLQueryGenerator.GetCreateTableQuery(table.TableName, columnNames);
				context.ExecuteNonQuery(query);
			}
		}
	}
}