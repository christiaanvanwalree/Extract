using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Extract
{
	public class TableModel
	{
		public readonly string name;
		public readonly ReadOnlyCollection<ColumnModel> columns;

		public int ColumnCount { get { return (columns == null) ? 0 : columns.Count; } }

		public TableModel(string name, List<ColumnModel> columns) {
			this.name = name;
			this.columns = new ReadOnlyCollection<ColumnModel>(columns);

		}
	}
}