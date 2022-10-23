using SqliteSharding.WebApp;
using SqliteSharding.WebApp.DatabaseStrategies.Sharding;
using SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider;
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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/single/{key}", FindValueByKey_SingleDatabase_Query.Handle);
app.MapGet("/sharded/{key}", FindValueByKey_ShardedDatabase_Query.Handle);
app.MapGet("/memory/{key}", FindValueByKey_InMemory_Query.Handle);
app.MapPost("/generate-models", GenerateModelsCommand.Handle);

Console.WriteLine("Starting service...");
app.Run("http://localhost:5000");