using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider
{
    public class SQLiteCommandProviderStrategy_NoTransaction : ISQLiteCommandProviderStrategy
    {
        public Task CommitAsync()
        {
            return Task.CompletedTask;
        }

        public async Task CreateCommandAsync(string connectionString, Func<SQLiteConnection, SQLiteCommand, Task> handler)
        {
            await CreateCommandAsync<object?>(connectionString, async (conn, command) => 
            {
                await handler.Invoke(conn, command);
                return null;
            });
        }

        public async Task<T> CreateCommandAsync<T>(string connectionString, Func<SQLiteConnection, SQLiteCommand, Task<T>> handler)
        {
            using(var conn = new SQLiteConnection(connectionString))
            using(var command = conn.CreateCommand())
            {
                await conn.OpenAsync();
                return await handler.Invoke(conn, command);
            }
        }
    }
}