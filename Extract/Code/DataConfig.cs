using Newtonsoft.Json;
using System;
using System.IO;

namespace Extract
{
	public static class DataConfig
	{

		public const string configFile = "config.json";
		public const string exportFolder = "exports\\";
		public const string bakExtension = ".bak";
		public const string csvExtension = ".csv";
		public const string zipExtension = ".zip";

		public static readonly DataType DatabaseType = DataType.Sql;
		public static readonly SqlServerConfig SqlServerConfig;
		public static readonly MySqlServerConfig MySqlServerConfig;

		static DataConfig() {
			StreamReader reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile));
			ConfigModel model = JsonConvert.DeserializeObject<ConfigModel>(reader.ReadToEnd());
			SqlServerConfig = model.SqlServerConfig;
			MySqlServerConfig = model.MySqlServerConfig;
		}

		private class ConfigModel
		{
			public readonly SqlServerConfig SqlServerConfig;
			public readonly MySqlServerConfig MySqlServerConfig;

			public ConfigModel(SqlServerConfig SqlServerConfig, MySqlServerConfig MySqlServerConfig) {
				this.SqlServerConfig = SqlServerConfig;
				this.MySqlServerConfig = MySqlServerConfig;
			}
		}
	}

	public class SqlServerConfig
	{
		public readonly string DataSource;
		public readonly string InitialCatalog;
		public readonly string UserId;
		public readonly string Password;
		public readonly string ConnectionTimeout;
		public readonly string DbSavePath;
		public readonly string SqlConnectionUrl = "Data Source={0}; Initial Catalog={1}; User id={2}; Password={3}; Connection Timeout={4};";

		public SqlServerConfig(string DataSource, string InitialCatalog, string UserId, string Password, string ConnectionTimeout, string DbSavePath) {
			this.DataSource = DataSource;
			this.InitialCatalog = InitialCatalog;
			this.UserId = UserId;
			this.Password = Password;
			this.ConnectionTimeout = ConnectionTimeout;
			this.DbSavePath = DbSavePath;
		}
	}

	public class MySqlServerConfig
	{
		public readonly string Server;
		public readonly string Port;
		public readonly string UserId;
		public readonly string Password;
		public readonly string DbSavePath;
		public readonly string MysqlConnectionUrl = "server={0}:{1}; uid={2}; pwd={3}; database={4};";

		public MySqlServerConfig(string Server, string Port, string UserId, string Password, string DbSavePath) {
			this.Server = Server;
			this.Port = Port;
			this.UserId = UserId;
			this.Password = Password;
			this.DbSavePath = DbSavePath;
		}
	}
}