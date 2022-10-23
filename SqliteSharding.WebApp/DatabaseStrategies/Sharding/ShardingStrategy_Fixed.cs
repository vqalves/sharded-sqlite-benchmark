using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqliteSharding.WebApp.Repositories.TestRepositoryStrategies;

namespace SqliteSharding.WebApp.DatabaseStrategies.Sharding
{
    public class ShardingStrategy_Fixed : IShardingStrategy
    {
        private readonly IShardingConfig Config;

        public ShardingStrategy_Fixed(IShardingConfig config)
        {
            this.Config = config;
        }

        public string GenerateConnectionString(string key)
        {
            var databaseName = Config.GetSqliteSingleDatabaseName();
            var databasePath = Config.GetSqliteDatabasePath(databaseName);
            var poolSize = Config.GetConnectionPoolSize();
            return Config.GenerateConnectionString(databasePath, poolSize);
        }
    }
}