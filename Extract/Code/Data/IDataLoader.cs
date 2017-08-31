
namespace Extract
{
	public interface IDataLoader
	{
		void Import(DataFile file);
		DataFile Export(string database);
	}
}
