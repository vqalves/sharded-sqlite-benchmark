using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.Helpers.DatabaseGeneratorModule
{
    public interface IDatabaseGeneratorConfig
    {
        int GetShardedDatabaseCount();
        string GetSqliteDatabasesFolder();
        string GetSqliteModelDatabaseName();
        string GetSqliteDatabasePath(string databaseName);
        string GetSqliteShardedDatabaseName(int i);
        string GetSqliteSingleDatabaseName();
    }
}