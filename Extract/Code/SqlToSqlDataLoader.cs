using Extract;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;

namespace Extract
{
	public class SqlToSqlDataLoader : IDataLoader
	{
		
		private readonly Server server;
		private readonly SqlConnection connection;
		private readonly string savePath = DataConfig.SqlServerConfig.DbSavePath;

		public SqlToSqlDataLoader(Server server) {
			if (server == null) throw new ArgumentNullException("server");

			this.server = server;
			this.connection = server.ConnectionContext.SqlConnectionObject;
		}

		public string Export(string database) {
			string fileName = Guid.NewGuid() + DataConfig.bakExtension;
			string relativePath = Path.Combine(DataConfig.exportFolder, fileName);
			string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

			Backup backup = new Backup();
			backup.Action = BackupActionType.Database;
			backup.Database = database;
			backup.Devices.AddDevice(absolutePath, DeviceType.File);
			backup.BackupSetName = fileName;
			backup.BackupSetDescription = database;
			backup.ExpirationDate = DateTime.Today.AddDays(100);
			backup.Initialize = false;
			backup.SqlBackup(server);

			return relativePath;
		}

		public void Load(DataFile file) {

			string relativeZipPath = string.Empty;
			CsvToSqlDataLoader loader = null;

			if (server.Databases.Contains(file.database)) {
				loader = new CsvToSqlDataLoader(server);
				relativeZipPath = loader.Export(file.database);
			}

			Unload(file);

			Restore restore = new Restore();
			BackupDeviceItem backup = new BackupDeviceItem(file.path, DeviceType.File);
			restore.Database = file.database;
			restore.Devices.Add(backup);
			
			RelocateFile relocateDataFile = new RelocateFile(LookupLogicalName(restore, "D"), string.Format(savePath, file.database, ".mdf"));
			RelocateFile relocateLogFile = new RelocateFile(LookupLogicalName(restore, "L"), string.Format(savePath, file.database, ".ldf"));

			restore.RelocateFiles.Add(relocateDataFile);
			restore.RelocateFiles.Add(relocateLogFile);
			restore.Action = RestoreActionType.Database;
			restore.ReplaceDatabase = false;
			restore.SqlRestore(server);

			if (!relativeZipPath.Equals(string.Empty)) {
				ZipArchive archive = ZipFile.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeZipPath));
				for (int i = 0; i < archive.Entries.Count; i++) {
					string destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataConfig.exportFolder, archive.Entries[i].FullName);
					archive.Entries[i].ExtractToFile(destinationPath);
					loader.Load(new DataFile(destinationPath, archive.Entries[i].FullName, file.database));
				}
			}
		}

		public void Unload(DataFile file) {
			if (server.Databases.Contains(file.database)) {
				server.KillDatabase(file.database);
			}
		}

		private string LookupLogicalName(Restore restore, string type) {
			DataTable table = restore.ReadFileList(server);
			DataRowCollection rows = table.Rows;
			DataColumnCollection columns = table.Columns;

			for (int i = 0; i < rows.Count; i++) {
				DataRow row = rows[i];
				bool typeMatched = false;
				string logicalName = string.Empty;

				for (int j = 0; j < columns.Count; j++) {
					DataColumn column = columns[j];

					if (column.ColumnName.Equals("LogicalName")) {
						logicalName = rows[i][j].ToString();
					} else if (column.ColumnName.Equals("Type")) {
						typeMatched = (rows[i][j].ToString() == type);
					}

					if (typeMatched && logicalName != string.Empty) {
						return logicalName;
					}
				}
			}
			throw new Exception("Could not find logical name");
		}
	}
}