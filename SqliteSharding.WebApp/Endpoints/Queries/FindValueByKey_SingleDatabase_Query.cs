using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqliteSharding.WebApp.DatabaseStrategies.Sharding;
using SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider;
using SqliteSharding.WebApp.Repositories;
using SqliteSharding.WebApp.Repositories.TestRepositoryStrategies;

namespace SqliteSharding.WebApp.Endpoints.Queries
{
    public class FindValueByKey_SingleDatabase_Query
    {
        private FindValueByKey_SingleDatabase_Query() { }

        public static async Task<IResult> Handle(
            [FromServices]IShardingConfig shardingConfig, 
            [FromServices]ISQLiteCommandProviderStrategy commandProvider, 
            
            [FromRoute]string key
        )
        {
            var shardingStrategy = new ShardingStrategy_Fixed(shardingConfig);
            var testRepository = new TestRepository_Sqlite(shardingStrategy, commandProvider);

            var result = await testRepository.FindValueAsync(key);
            return Results.Text(result ?? string.Empty);
        }
    }
}