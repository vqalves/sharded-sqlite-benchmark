using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider
{
    public interface ISQLiteCommandProviderStrategy
    {
        Task CommitAsync();
        Task CreateCommandAsync(string connectionString, Func<SQLiteConnection, SQLiteCommand, Task> handler);
        Task<T> CreateCommandAsync<T>(string connectionString, Func<SQLiteConnection, SQLiteCommand, Task<T>> handler);
    }
}