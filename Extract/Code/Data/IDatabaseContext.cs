using System.Collections;
using System.Data;

namespace Extract
{
	public interface IDatabaseContext
	{
		bool DatabaseExists(string database);
		void CreateDatabase(string database);
		void DropDatabase(string database);
	}
}