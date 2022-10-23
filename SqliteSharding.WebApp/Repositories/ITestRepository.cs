using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.Repositories
{
    public interface ITestRepository
    {
        Task<string?> FindValueAsync(string key);
        Task InsertAsync(string key, string value);
    }
}