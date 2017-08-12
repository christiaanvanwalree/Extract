using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Extract
{
	public class SqlDataController : IDataController
	{

		private readonly string database;
		private readonly Server server;
		private readonly SqlConnection connection;

		public SqlDataController(Server server) {
			if (server == null) throw new ArgumentNullException("server");

			this.server = server;
			this.connection = server.ConnectionContext.SqlConnectionObject;
			this.database = server.ConnectionContext.SqlConnectionObject.Database;
		}


		public DataModel GetData() {
			DataModel dataModel = new DataModel(database, GetTables());
			return dataModel;
		}


		public DataModel GetMetaData() {
			DataModel dataModel = new DataModel(database, GetMetaTables());
			return dataModel;
		}


		public List<TableModel> GetTables() {
			if (!IsDataAvailable(database)) throw new Exception("database not available");

			List<TableModel> tables = new List<TableModel>();
			TableCollection collection = server.Databases[database].Tables;

			for (int i = 0; i < collection.Count; i++) {
				string tableName = collection[i].Name;
				tables.Add(new TableModel(tableName, GetColumns(tableName)));
			}

			return tables;
		}


		public List<TableModel> GetMetaTables() {
			if (!IsDataAvailable(database)) throw new Exception("database not available");

			List<TableModel> tables = new List<TableModel>();
			TableCollection collection = server.Databases[database].Tables;

			for (int i = 0; i < collection.Count; i++) {
				string tableName = collection[i].Name;
				tables.Add(new TableModel(tableName, GetMetaColumns(tableName)));
			}

			return tables;
		}


		public List<ColumnModel> GetColumns(string table) {
			if (!IsDataAvailable(database)) throw new ArgumentException(database);
			if (!TableExists(database, table)) throw new ArgumentException(table);

			List<ColumnModel> columns = new List<ColumnModel>();
			List<string> columnNames = new List<string>();
			Dictionary<string, string> columnTypes = new Dictionary<string, string>();
			Dictionary<string, List<string>> columnValues = new Dictionary<string, List<string>>();

			ColumnCollection collection = server.Databases[database].Tables[table].Columns;
			for (int i = 0; i < collection.Count; i++) {
				columnNames.Add(collection[i].Name);
			}

			SqlCommand command = new SqlCommand("SELECT * FROM [" + table + "];", connection);
			SqlDataReader reader = command.ExecuteReader();

			while (reader.Read() ) { 
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
				columns.Add(new ColumnModel(columnName, columnTypes[columnName].ToString(), columnValues[columnName]));
			}

			return columns;
		}


		public List<ColumnModel> GetMetaColumns(string table) {
			if (!IsDataAvailable(database)) throw new ArgumentException(database);
			if (!TableExists(database, table)) throw new ArgumentException(table);

			List<ColumnModel> columns = new List<ColumnModel>();
			ColumnCollection collection = server.Databases[database].Tables[table].Columns;

			for (int i = 0; i < collection.Count; i++) {
				columns.Add(new ColumnModel(collection[i].Name, collection[i].DataType.ToString()));
			}

			return columns;
		}


		public bool IsDataAvailable(string id) {
			return server.Databases.Contains(id) && server.Databases[id].IsAccessible;
		}


		public bool TableExists(string id, string table) {
			return IsDataAvailable(id) && server.Databases[id].Tables.Contains(table);
		}


		public bool ColumnExists(string id, string table, string column) {
			return TableExists(id, table) && server.Databases[id].Tables[table].Columns.Contains(column);
		}
	}
}