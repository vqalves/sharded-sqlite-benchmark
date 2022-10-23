using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.Repositories.TestRepositoryStrategies
{
    public class TestRepository_InMemory : ITestRepository
    {
        private static Dictionary<string, string> Values = new Dictionary<string, string>();

        public async Task<string?> FindValueAsync(string key)
        {
            var value = Values.GetValueOrDefault(key);
            return await Task.FromResult(value);
        }

        public async Task InsertAsync(string key, string value)
        {
            Values.Add(key, value);
            await Task.CompletedTask;
        }
    }
}