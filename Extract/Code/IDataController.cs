using System.Collections.Generic;

namespace Extract
{
	public interface IDataController
	{
		bool IsDataAvailable(string id);
		bool TableExists(string id, string table);
		bool ColumnExists(string id, string table, string column);

		DataModel GetMetaData();
		DataModel GetData();
		List<TableModel> GetTables();
		List<ColumnModel> GetColumns(string table);
	}
}
