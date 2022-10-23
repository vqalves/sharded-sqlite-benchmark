using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.DatabaseStrategies.Sharding
{
    public interface IShardingConfig
    {
        int GetShardedDatabaseCount();
        string GetSqliteShardedDatabaseName(int index);
        string GetSqliteDatabasePath(string databaseName);
        int GetConnectionPoolSize();
        string GenerateConnectionString(string databasePath, int poolSize);
        string GetSqliteSingleDatabaseName();
    }
}