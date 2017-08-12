using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Extract
{
	public class ColumnModel
	{
		public readonly string name;
		public readonly string type;
		public readonly ReadOnlyCollection<string> values;

		public ColumnModel(string name, string type, List<string> values = null) {
			this.name = name;
			this.type = type;
			this.values = new ReadOnlyCollection<string>((values == null) ? new List<string>() : values);
		}
	}
}