using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.IO;

namespace Extract
{
	public class SQLBAKDataLoader : IDataLoader
	{
		
		private readonly SQLServerContext context;
		private readonly string savePath = DataConfig.SQLConfig.DbSavePath;


		public SQLBAKDataLoader(SQLServerContext context) {
			if (context == null) throw new ArgumentNullException("context");

			this.context = context;
		}


		public DataFile Export(string database) {
			string fileName = Guid.NewGuid() + DataConfig.bakExt;
			string relativePath = Path.Combine(DataConfig.exportDir, fileName);
			string absolutePath = Path.Combine(DataConfig.baseDir, relativePath);

			Backup backup = new Backup();
			backup.Action = BackupActionType.Database;
			backup.Database = database;
			backup.Devices.AddDevice(absolutePath, DeviceType.File);
			backup.BackupSetName = fileName;
			backup.BackupSetDescription = database;
			backup.ExpirationDate = DateTime.Today.AddDays(100);
			backup.Initialize = false;
			backup.SqlBackup(SQLServerContext.Server);

			DataFile file = new DataFile(relativePath, fileName, database);
			return file;
		}


		public void Import(DataFile file) {
			if (context.DatabaseExists(file.database)) throw new InvalidOperationException("database already exists");

			Restore restore = new Restore();
			BackupDeviceItem backup = new BackupDeviceItem(file.path, DeviceType.File);
			restore.Database = file.database;
			restore.Devices.Add(backup);
			
			RelocateFile relocateDataFile = new RelocateFile(LookupLogicalName(restore, "D"), string.Format(savePath, file.database, DataConfig.mdfExt));
			RelocateFile relocateLogFile = new RelocateFile(LookupLogicalName(restore, "L"), string.Format(savePath, file.database, DataConfig.ldfExt));

			restore.RelocateFiles.Add(relocateDataFile);
			restore.RelocateFiles.Add(relocateLogFile);
			restore.Action = RestoreActionType.Database;
			restore.ReplaceDatabase = false;
			restore.SqlRestore(SQLServerContext.Server);

			context.ChangeDatabase(file.database);
		}


		private string LookupLogicalName(Restore restore, string type) {
			DataTable table = restore.ReadFileList(SQLServerContext.Server);
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