using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.DatabaseStrategies.Sharding
{
    public interface IShardingStrategy
    {
        string GenerateConnectionString(string key);
    }
}