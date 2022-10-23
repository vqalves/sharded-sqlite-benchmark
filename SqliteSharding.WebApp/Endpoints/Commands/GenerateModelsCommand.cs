using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqliteSharding.WebApp.Helpers.DatabaseGeneratorModule;

namespace SqliteSharding.WebApp.Endpoints.Commands
{
    public class GenerateModelsCommand
    {
        private GenerateModelsCommand() { }

        public static async Task<IResult> Handle([FromServices]DatabaseGenerator databaseGenerator)
        {
            await databaseGenerator.GenerateModelsAsync(1_000_000);
            return Results.Ok();
        }
    }
}