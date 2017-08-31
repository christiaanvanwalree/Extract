using System.IO;

namespace Extract
{
	public class DataFile
	{
		public readonly string guid;
		public readonly string fileName;
		public readonly string database;
		public readonly string path;
		public readonly string relativePath;
		public readonly DataType dataType;


		public DataFile(string path, string fileName, string database) {
			this.path = path;
			fileName = fileName.Replace("\"", string.Empty);
			this.fileName = (fileName.Contains(".")) ? fileName.Remove(fileName.IndexOf('.')) : fileName;
			this.guid = path.Substring(path.LastIndexOf('\\') + 1).Replace("BodyPart_", string.Empty);
			this.database = (string.IsNullOrWhiteSpace(database)) ? guid : database;
			this.relativePath = path.Remove(0, DataConfig.baseDir.Length);
			this.dataType = GetFileType(fileName);
			
		}


		public void Delete() {
			File.Delete(path);
		}


		private DataType GetFileType(string fileName) {
			if (fileName.EndsWith(DataConfig.csvExt)) {
				return DataType.CSV;
			} else if (fileName.EndsWith(DataConfig.bakExt)) {
				return DataType.BAK;
			} else if (fileName.EndsWith(DataConfig.xmlExt)) {
				return DataType.XML;
			} else if (fileName.EndsWith(DataConfig.zipExt)) {
				return DataType.Archive;
			} else {
				throw new InvalidDataException("filetype not recognized");
			}
		}
	}
}