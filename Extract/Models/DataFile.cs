using System.IO;

namespace Extract
{
	public class DataFile
	{
		public readonly string id;
		public readonly string fileName;
		public readonly string database;
		public readonly string path;
		public readonly DataType type;

		public DataFile(string path, string fileName, string database = null) {
			this.id = path.Substring(path.LastIndexOf('\\') + 1).Replace("BodyPart_", string.Empty);

			fileName = fileName.Replace("\"", string.Empty);
			this.fileName = (fileName.Contains(".")) ? fileName.Remove(fileName.IndexOf('.')) : fileName;

			this.database = (database == null) ? id : database;
			this.path = path;
			
			if (fileName.EndsWith(".csv")) {
				this.type = DataType.Csv;
			} else if (fileName.EndsWith(".bak")) {
				this.type = DataType.Sql;
			} else {
				throw new InvalidDataException("filetype not recognized");
			}
		}

		public void Delete() {
			File.Delete(path);
		}
	}
}