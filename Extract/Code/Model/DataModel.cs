using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Extract
{
	public class DataModel
	{
		public readonly string database;
		public readonly ReadOnlyCollection<TableModel> tables;

		public DataModel(string database, List<TableModel> tables) {
			this.database = database;
			this.tables = new ReadOnlyCollection<TableModel>(tables);
		}
	}
}