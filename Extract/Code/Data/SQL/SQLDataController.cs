using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Extract
{
	public class SQLDataController : IDataController
	{
		
		private readonly SQLDatabaseController controller;

		public SQLDataController(SQLDatabaseController controller) {
			if (controller == null) throw new ArgumentNullException("controller");

			this.controller = controller;
		}


		public DataModel GetData() {
			DataModel dataModel = new DataModel(controller.DatabaseName, GetTables());
			return dataModel;
		}


		public DataModel GetMetaData() {
			DataModel dataModel = new DataModel(controller.DatabaseName, GetMetaTables());
			return dataModel;
		}


		public List<TableModel> GetTables() {
			List<TableModel> tables = new List<TableModel>();
			TableCollection collection = controller.Tables;

			for (int i = 0; i < collection.Count; i++) {
				string tableName = collection[i].Name;
				tables.Add(new TableModel(tableName, GetColumns(tableName)));
			}

			return tables;
		}


		public List<TableModel> GetMetaTables() {
			List<TableModel> tables = new List<TableModel>();
			TableCollection collection = controller.Tables;

			for (int i = 0; i < collection.Count; i++) {
				string tableName = collection[i].Name;
				tables.Add(new TableModel(tableName, GetMetaColumns(tableName)));
			}

			return tables;
		}


		public List<ColumnModel> GetColumns(string table) {
			List<ColumnModel> columns = new List<ColumnModel>();
			ColumnCollection collection = controller.Tables[table].Columns;
			controller.ExecuteReader("SELECT * FROM [" + table + "];", (reader) => DataModelController.CreateColumnModels(reader, collection, columns));
			return columns;
		}


		public List<ColumnModel> GetMetaColumns(string table) {
			List<ColumnModel> columns = new List<ColumnModel>();
			ColumnCollection collection = controller.Tables[table].Columns;

			for (int i = 0; i < collection.Count; i++) {
				columns.Add(new ColumnModel(collection[i].Name, collection[i].DataType.ToString()));
			}

			return columns;
		}
	}
}