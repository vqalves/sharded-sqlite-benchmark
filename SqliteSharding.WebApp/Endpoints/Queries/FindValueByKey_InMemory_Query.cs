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
    public class FindValueByKey_InMemory_Query
    {
        private FindValueByKey_InMemory_Query() { }

        public static async Task<IResult> Handle(
            [FromRoute]string key
        )
        {
            var testRepository = new TestRepository_InMemory();

            var result = await testRepository.FindValueAsync(key);
            return Results.Text(result ?? string.Empty);
        }
    }
}