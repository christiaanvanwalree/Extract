using System.IO;

namespace Extract
{
	public interface IDataLoader
	{
		string Export(string database);
		void Load(DataFile file);
	}
}
