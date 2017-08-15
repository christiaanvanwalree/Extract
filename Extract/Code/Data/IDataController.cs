using System.Collections.Generic;

namespace Extract
{
	public interface IDataController
	{
		DataModel GetMetaData();
		DataModel GetData();
		List<TableModel> GetTables();
		List<ColumnModel> GetColumns(string table);
	}
}
