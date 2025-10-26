using System;
using System.IO;
using Newtonsoft.Json;

namespace SyncVerseStudio.Helpers
{
    public static class DatabaseConnectionManager
    {
        private const string ConfigFile = "dbconfig.json";
        private static string _connectionString;

        public static string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(_connectionString))
                return _connectionString;

            try
            {
                if (File.Exists(ConfigFile))
                {
                    var json = File.ReadAllText(ConfigFile);
                    var config = JsonConvert.DeserializeObject<DatabaseConfig>(json);
                    if (config != null && !string.IsNullOrEmpty(config.ConnectionString))
                    {
                        _connectionString = config.ConnectionString;
                        return _connectionString;
                    }
                }
            }
            catch { }

            return null;
        }

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            try
            {
                var config = new DatabaseConfig { ConnectionString = connectionString };
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
            }
            catch { }
        }

        private class DatabaseConfig
        {
            public string ConnectionString { get; set; }
        }
    }
}
