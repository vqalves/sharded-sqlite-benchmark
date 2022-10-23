using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqliteSharding.WebApp.Repositories.TestRepositoryStrategies;

namespace SqliteSharding.WebApp.DatabaseStrategies.Sharding
{
    public class ShardingStrategy_Dynamic : IShardingStrategy
    {
        private readonly IShardingConfig Config;
        private readonly ShardingHashCalculator ShardingHashCalculator;

        public ShardingStrategy_Dynamic(IShardingConfig config, ShardingHashCalculator shardingHashCalculator)
        {
            this.Config = config;
            this.ShardingHashCalculator = shardingHashCalculator;
        }

        public string GenerateDatabaseName(string key)
        {
            var index = ShardingHashCalculator.CalculateHash(key, Config.GetShardedDatabaseCount());
            return Config.GetSqliteShardedDatabaseName(index);
        }

        public string GenerateConnectionString(string key)
        {
            var databaseName = GenerateDatabaseName(key);
            var databasePath = Config.GetSqliteDatabasePath(databaseName);
            var poolSize = Config.GetConnectionPoolSize() / Config.GetShardedDatabaseCount();

            return Config.GenerateConnectionString(databasePath, poolSize);
        }
    }
}