using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using SqliteSharding.WebApp.DatabaseStrategies.Sharding;
using SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider;

namespace SqliteSharding.WebApp.Repositories.TestRepositoryStrategies
{
    public class TestRepository_Sqlite : ITestRepository
    {
        private readonly IShardingStrategy Sharding;
        private readonly ISQLiteCommandProviderStrategy CommandProvider;

        public TestRepository_Sqlite(IShardingStrategy sharding, ISQLiteCommandProviderStrategy commandProvider)
        {
            this.Sharding = sharding;
            this.CommandProvider = commandProvider ?? new SQLiteCommandProviderStrategy_NoTransaction();
        }

        private string GetConnectionString(string key) => Sharding.GenerateConnectionString(key);

        public async Task<string?> FindValueAsync(string key)
        {
            var connectionString = GetConnectionString(key);

            return await CommandProvider.CreateCommandAsync(connectionString, async (connection, command) => 
            {
                command.CommandText = "SELECT Value FROM Tuple WHERE Key = @Key";
                command.Parameters.AddWithValue("@Key", key);

                var result = await command.ExecuteScalarAsync();
                return result?.ToString();
            });
        }

        public async Task InsertAsync(string key, string value)
        {
            var connectionString = GetConnectionString(key);

            await CommandProvider.CreateCommandAsync(connectionString, async (connection, command) => 
            {
                command.CommandText = "INSERT INTO Tuple(Key, Value) VALUES (@Key, @Value)";
                command.Parameters.AddWithValue("@Key", key);
                command.Parameters.AddWithValue("@Value", value);

                await command.ExecuteNonQueryAsync();
            });
        }
    }
}