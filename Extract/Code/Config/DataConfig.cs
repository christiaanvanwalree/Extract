using Newtonsoft.Json;
using System;
using System.IO;

namespace Extract
{
	public static class DataConfig
	{

		public static readonly string bakExt = ".bak";
		public static readonly string mdfExt = ".mdf";
		public static readonly string ldfExt = ".ldf";
		public static readonly string csvExt = ".csv";
		public static readonly string xmlExt = ".xml";
		public static readonly string xlsExt = ".xls";
		public static readonly string zipExt = ".zip";

		public static readonly string configPath = "config.json";
		public static readonly string exportDir = "exports";
		public static readonly string baseDir = AppDomain.CurrentDomain.BaseDirectory;
		public static readonly string uploadDir = "~/App_Data/uploads";

		public static readonly DataType DatabaseType = DataType.SQL;
		public static readonly SQLConfig SQLConfig;
		public static readonly MySQLConfig MySQLConfig;

		static DataConfig() {
			StreamReader reader = new StreamReader(Path.Combine(baseDir, configPath));
			ConfigModel model = JsonConvert.DeserializeObject<ConfigModel>(reader.ReadToEnd());
			SQLConfig = model.SQLServerConfig;
			MySQLConfig = model.MySQLServerConfig;
		}

		private class ConfigModel
		{
			public readonly SQLConfig SQLServerConfig;
			public readonly MySQLConfig MySQLServerConfig;

			public ConfigModel(SQLConfig SQLServerConfig, MySQLConfig MySQLServerConfig) {
				this.SQLServerConfig = SQLServerConfig;
				this.MySQLServerConfig = MySQLServerConfig;
			}
		}
	}

	public class SQLConfig
	{
		public readonly string DataSource;
		public readonly string InitialCatalog;
		public readonly string UserId;
		public readonly string Password;
		public readonly string ConnectionTimeout;
		public readonly string DbSavePath;
		public readonly string connectionString = "Data Source={0}; Initial Catalog={1}; User id={2}; Password={3}; Connection Timeout={4};";

		public SQLConfig(string DataSource, string InitialCatalog, string UserId, string Password, string ConnectionTimeout, string DbSavePath) {
			this.DataSource = DataSource;
			this.InitialCatalog = InitialCatalog;
			this.UserId = UserId;
			this.Password = Password;
			this.ConnectionTimeout = ConnectionTimeout;
			this.DbSavePath = DbSavePath;
		}
	}

	public class MySQLConfig
	{
		public readonly string Server;
		public readonly string Port;
		public readonly string UserId;
		public readonly string Password;
		public readonly string DbSavePath;
		public readonly string MysqlConnectionUrl = "server={0}:{1}; uid={2}; pwd={3}; database={4};";

		public MySQLConfig(string Server, string Port, string UserId, string Password, string DbSavePath) {
			this.Server = Server;
			this.Port = Port;
			this.UserId = UserId;
			this.Password = Password;
			this.DbSavePath = DbSavePath;
		}
	}
}