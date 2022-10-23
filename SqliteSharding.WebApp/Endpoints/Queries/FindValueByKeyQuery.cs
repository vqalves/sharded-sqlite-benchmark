using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqliteSharding.WebApp.Repositories;

namespace SqliteSharding.WebApp.Endpoints.Queries
{
    public class FindValueByKeyQuery
    {
        public static async Task<IResult> Handle([FromServices]ITestRepository testRepository, [FromRoute]string key)
        {
            var result = await testRepository.FindValueAsync(key);
            return Results.Text(result ?? string.Empty);
        }
    }
}