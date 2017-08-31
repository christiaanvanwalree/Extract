using System;

namespace Extract
{
	public class SQLQueryGenerator
	{


		public static string EncloseInBrackets(string text) {
			return "[" + text + "]";
		}


		public static string GetSelectQuery(string table) {
			return "SELECT * FROM " + EncloseInBrackets(table) + ";";
		}


		public static string GetSelectQuery(string table, string[] columns) {
			if (string.IsNullOrWhiteSpace(table)) throw new ArgumentNullException("table");
			if (columns == null || columns.Length == 0) throw new ArgumentNullException("columns");

			string query = "SELECT ";
			for (int i = 0; i < columns.Length; i++) {
				if (i > 0) {
					query += ", ";
				}
				query += columns[i];
			}
			query += " FROM " + EncloseInBrackets(table) + ";";

			return query;
		}


		public static string GetCreateDatabaseQuery(string database) {
			return "CREATE DATABASE " + EncloseInBrackets(database) + ";";
		}


		public static string GetDropDatabaseQuery(string database) {
			return "DROP DATABASE " + EncloseInBrackets(database) + ";";
		}


		public static string GetCreateTableQuery(string table, string[] columns) {
			if (string.IsNullOrWhiteSpace(table)) throw new ArgumentNullException("table");
			if (columns == null || columns.Length == 0) throw new ArgumentNullException("columns");

			string query = "CREATE TABLE " + EncloseInBrackets(table) + " (";
			for (int i = 0; i < columns.Length; i++) {
				if (i > 0) {
					query += ", ";
				}
				query += EncloseInBrackets(columns[i]) + " nvarchar(MAX)" ;
			}
			query += ");";

			return query;
		}

		public static string GetCreateTableQuery(string table, string[] columns, string[] datatypes) {
			if (string.IsNullOrWhiteSpace(table)) throw new ArgumentNullException("table");
			if (columns == null) throw new ArgumentNullException("columns");
			if (columns.Length > datatypes.Length) throw new ArgumentException("all columns should have a datatype");

			string query = "CREATE TABLE " + EncloseInBrackets(table) + " (";

			for (int i = 0; i < columns.Length; i++) {
				if (i > 0) {
					query += ", ";
				}
				query += EncloseInBrackets(columns[i]) + " " + datatypes[i];
			}
			query += ");";

			return query;
		}
	}
}