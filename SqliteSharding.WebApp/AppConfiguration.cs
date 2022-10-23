using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SqliteSharding.WebApp.DatabaseStrategies.Sharding;
using SqliteSharding.WebApp.Helpers;
using SqliteSharding.WebApp.Helpers.DatabaseGeneratorModule;
using SqliteSharding.WebApp.Helpers.RandomCodeGeneratorModule;

namespace SqliteSharding.WebApp
{
    public class AppConfiguration : IShardingHashConfig, IDatabaseGeneratorConfig, IRandomCodeGeneratorConfig, IShardingConfig
    {
        private readonly string DLLFolder;
        public AppConfiguration()
        {
            var dllPath = Assembly.GetExecutingAssembly().Location;
            this.DLLFolder = Path.GetDirectoryName(dllPath)!;
        }

        public string GetSqliteDatabasesFolder() => Path.Combine(DLLFolder, "SqliteDatabases");
        public string GetSqliteDatabasePath(string databaseName) => Path.Combine(GetSqliteDatabasesFolder(), databaseName);
        public string GetSqliteModelDatabaseName() => "Model.sqlite";
        public string GetSqliteSingleDatabaseName() => "Single.sqlite";
        public string GetSqliteShardedDatabaseName(int index) => $"Sharded_{index}.sqlite";
        public string GenerateConnectionString(string filePath, int poolSize) => $"Data Source={filePath};Version=3;Pooling=True;Max Pool Size={poolSize};";
        public IEnumerable<char> GetAvailableCodeCharacters() => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public int GetShardedDatabaseCount() => 10;
        public int GetConnectionPoolSize() => 50;
    }
}