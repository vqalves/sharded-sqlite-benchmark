using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqliteSharding.WebApp.DatabaseStrategies.Sharding;
using SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider;
using SqliteSharding.WebApp.Helpers.RandomCodeGeneratorModule;
using SqliteSharding.WebApp.Repositories.TestRepositoryStrategies;

namespace SqliteSharding.WebApp.Helpers.DatabaseGeneratorModule
{
    public class DatabaseGenerator
    {
        private readonly IDatabaseGeneratorConfig DatabaseGeneratorConfig;
        private readonly RandomCodeGenerator CodeGenerator;
        private readonly ShardingHashCalculator ShardingHashCalculator;
        private readonly IShardingConfig ShardingConfig;

        public DatabaseGenerator(IDatabaseGeneratorConfig databaseGeneratorConfig, ShardingHashCalculator shardingHashCalculator, IShardingConfig shardingConfig, RandomCodeGenerator codeGenerator)
        {
            this.ShardingConfig = shardingConfig;
            this.ShardingHashCalculator = shardingHashCalculator;
            this.DatabaseGeneratorConfig = databaseGeneratorConfig;
            this.CodeGenerator = codeGenerator;
        }

        private void CreateDatabaseFiles()
        {
            var databaseCount = DatabaseGeneratorConfig.GetShardedDatabaseCount();
            var databaseFolder = DatabaseGeneratorConfig.GetSqliteDatabasesFolder();
            var modelFilePath = Path.Combine(databaseFolder, DatabaseGeneratorConfig.GetSqliteModelDatabaseName());
            
            // Create files
            var singleDatabaseName = DatabaseGeneratorConfig.GetSqliteSingleDatabaseName();
            var singleDatabasePath = DatabaseGeneratorConfig.GetSqliteDatabasePath(singleDatabaseName);
            File.Copy(modelFilePath, singleDatabasePath, true);

            for(var i = 0; i < databaseCount; i++)
            {
                var shardedDatabaseName = DatabaseGeneratorConfig.GetSqliteShardedDatabaseName(i);
                var shardedDatabasePath = DatabaseGeneratorConfig.GetSqliteDatabasePath(shardedDatabaseName);
                File.Copy(modelFilePath, shardedDatabasePath, true);
            }
        }

        private async Task InsertDataIntoDatabasesAsync(int elementsCount)
        {
            var transactionStrategy = new SQLiteCommandProviderStrategy_TransactionOpen();
            
            var fixedShardingDatabase = new ShardingStrategy_Fixed(ShardingConfig);
            var fixedRepository = new TestRepository_Sqlite(fixedShardingDatabase, transactionStrategy);

            var dynamicShardingStrategy = new ShardingStrategy_Dynamic(ShardingConfig, ShardingHashCalculator);
            var shardedRepository = new TestRepository_Sqlite(dynamicShardingStrategy, transactionStrategy);

            for(var i = 0; i < elementsCount; i++)
            {
                var key = CodeGenerator.GetRandomCode(8);
                var value = CodeGenerator.GetRandomCode(50);

                var insertShardedTask = shardedRepository.InsertAsync(key, value);
                var insertFixedTask = fixedRepository.InsertAsync(key, value);

                await Task.WhenAll(new [] { insertShardedTask, insertFixedTask });
            }

            await transactionStrategy.CommitAsync();
        }

        public async Task GenerateModelsAsync(int elementsCount)
        {
            CreateDatabaseFiles();
            await InsertDataIntoDatabasesAsync(elementsCount);
        }
    }
}