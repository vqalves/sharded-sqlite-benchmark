using SqliteSharding.WebApp;
using SqliteSharding.WebApp.Repositories.TestRepositoryStrategies;
using SqliteSharding.WebApp.DatabaseStrategies.Sharding;
using SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider;
using SqliteSharding.WebApp.Repositories;
using SqliteSharding.WebApp.Helpers.RandomCodeGeneratorModule;
using SqliteSharding.WebApp.Helpers.DatabaseGeneratorModule;
using SqliteSharding.WebApp.Endpoints.Commands;
using SqliteSharding.WebApp.Endpoints.Queries;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var config = new AppConfiguration();
var shardingHashCalculator = new ShardingHashCalculator(config);
var shardingStrategy = new ShardingStrategy_Dynamic(config, shardingHashCalculator);

builder.Services.AddSingleton<IShardingHashConfig>(x => config);
builder.Services.AddSingleton<IDatabaseGeneratorConfig>(x => config);
builder.Services.AddSingleton<IRandomCodeGeneratorConfig>(x => config);
builder.Services.AddSingleton<IShardingConfig>(x => config);

builder.Services.AddSingleton<ShardingHashCalculator>();
builder.Services.AddSingleton<RandomCodeGenerator>();
builder.Services.AddSingleton<DatabaseGenerator>();

builder.Services.AddScoped<ISQLiteCommandProviderStrategy, SQLiteCommandProviderStrategy_NoTransaction>();


Console.WriteLine("Welcome!");
Console.WriteLine($"SQLite databases mapped to folder '{Path.GetFullPath(config.GetSqliteDatabasesFolder())}'");
Console.WriteLine("Execute /generate-models at least once to populate the databases before running tests");
Console.WriteLine("Use /{key} to execute the lookup operation");

if(args.Contains("--single"))
{
    builder.Services.AddScoped<IShardingStrategy, ShardingStrategy_Fixed>();
    builder.Services.AddScoped<ITestRepository, TestRepository_Sqlite>();
}
else if(args.Contains("--sharded"))
{
    builder.Services.AddScoped<IShardingStrategy, ShardingStrategy_Dynamic>();
    builder.Services.AddScoped<ITestRepository, TestRepository_Sqlite>();
}
else if(args.Contains("--memory"))
{
    builder.Services.AddScoped<ITestRepository, TestRepository_InMemory>();
}
else
{
    throw new Exception("Error: No persistence mode specified. Add parameter --single to run under a single database, --sharded to run a sharded database or --memory to run in-memory");
}

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/{key}", FindValueByKeyQuery.Handle);
app.MapPost("/generate-models", GenerateModelsCommand.Handle);

app.Run("http://localhost:5000");

Console.WriteLine("Service started");