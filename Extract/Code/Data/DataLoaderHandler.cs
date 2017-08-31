using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;

namespace Extract
{
	public class DataLoaderHandler 
	{
		private readonly IDatabaseContext context;

		public DataLoaderHandler(IDatabaseContext context) {
			if (context == null) throw new ArgumentNullException("context");
			this.context = context;
		}


		public void ImportData(DataFile file, bool isReimport = false) {

			DataFile export = null;

			if (!isReimport && context.DatabaseExists(file.database)) {
				export = DefaultDataExport(file.database, true);
			}

			IDataLoader loader = DataLoaderFactory.CreateDataLoader(file.dataType, context);
			loader.Import(file);

			if (export == null) return;

			List<DataFile> dataFiles = new List<DataFile>() { export };
			if (export.dataType == DataType.Archive) {
				dataFiles = ExtractZip(export);
			}

			for (int i = 0; i < dataFiles.Count; i++) {
				ImportData(dataFiles[i], true); //recursive reimport
			}
		}


		public DataFile DefaultDataExport(string database, bool dropDatabase = false) {
			IDataLoader loader = DataLoaderFactory.CreateDefaultExporter(database, context);
			DataFile export = loader.Export(database);
			if (dropDatabase) {
				context.DropDatabase(database);
			}
			return export;
		}


		public DataFile ExportData(DataType dataType, string database, bool dropDatabase = false) {
			IDataLoader loader = DataLoaderFactory.CreateDataLoader(dataType, context);
			DataFile export = loader.Export(database);
			if (dropDatabase) {
				context.DropDatabase(database);
			}
			return export;
		}


		private List<DataFile> ExtractZip(DataFile zip) {

			List<DataFile> dataFiles = new List<DataFile>();
			ZipArchive archive = ZipFile.OpenRead(zip.path);
			ReadOnlyCollection<ZipArchiveEntry> entries = archive.Entries;

			string directory = zip.path.Replace(DataConfig.zipExt, string.Empty);
			archive.ExtractToDirectory(directory);

			for (int i = 0; i < entries.Count; i++) {
				ZipArchiveEntry entry = entries[i];
				dataFiles.Add(new DataFile(Path.Combine(directory, entry.FullName), entry.Name, zip.database));
			}

			return dataFiles;
		}
	}
}