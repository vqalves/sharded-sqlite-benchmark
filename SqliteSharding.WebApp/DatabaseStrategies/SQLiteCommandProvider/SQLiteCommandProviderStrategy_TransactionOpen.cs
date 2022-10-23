using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SqliteSharding.WebApp.DatabaseStrategies.SQLiteCommandProvider
{
    public class SQLiteCommandProviderStrategy_TransactionOpen : ISQLiteCommandProviderStrategy, IDisposable
    {
        private readonly Dictionary<string, SQLiteConnection> ConnectionByString;
        private readonly Dictionary<SQLiteConnection, SQLiteTransaction> TransactionByConnection;
        
        public SQLiteCommandProviderStrategy_TransactionOpen()
        {
            this.ConnectionByString = new Dictionary<string, SQLiteConnection>();
            this.TransactionByConnection = new Dictionary<SQLiteConnection, SQLiteTransaction>();
        }

        private async Task<IEnumerable<Exception>> RollbackTransactionsAsync()
        {
            var commitTasks = TransactionByConnection.Values.Select(async transaction => 
            {
                Exception? unhandledException = null;

                try { await transaction.RollbackAsync(); }
                catch(Exception ex) { unhandledException = ex; }

                return unhandledException;
            });

            var unhandledExceptions = await Task.WhenAll(commitTasks);
            TransactionByConnection.Clear();

            return unhandledExceptions.Where(x => x != null).Cast<Exception>();
        }

        private async Task<IEnumerable<Exception>> CommitTransactionsAsync()
        {
            var commitTasks = TransactionByConnection.Values.Select(async transaction => 
            {
                Exception? unhandledException = null;

                try { await transaction.CommitAsync(); }
                catch(Exception ex) { unhandledException = ex; }

                return unhandledException;
            });

            var unhandledExceptions = await Task.WhenAll(commitTasks);
            TransactionByConnection.Clear();

            return unhandledExceptions.Where(x => x != null).Cast<Exception>();
        }

        private async Task<IEnumerable<Exception>> CloseConnectionsAsync()
        {
            var commitTasks = ConnectionByString.Values.Select(async transaction => 
            {
                Exception? unhandledException = null;

                try { await transaction.CloseAsync(); }
                catch(Exception ex) { unhandledException = ex; }

                return unhandledException;
            });

            var unhandledExceptions = await Task.WhenAll(commitTasks);
            ConnectionByString.Clear();

            return unhandledExceptions.Where(x => x != null).Cast<Exception>();
        }

        public async Task CommitAsync()
        {
            var transactionExceptions = await CommitTransactionsAsync();
            var connectionExceptions = await CloseConnectionsAsync();

            // Throw an exception if any problem happened
            var exceptions = transactionExceptions.Concat(connectionExceptions);
            var firstException = exceptions.FirstOrDefault();
            if(firstException != null) throw firstException;
        }

        public async Task CreateCommandAsync(string connectionString, Func<SQLiteConnection, SQLiteCommand, Task> handler)
        {
            await CreateCommandAsync<object?>(connectionString, async (conn, command) => 
            {
                await handler.Invoke(conn, command);
                return null;
            });
        }

        public async Task<T> CreateCommandAsync<T>(string connectionString, Func<SQLiteConnection, SQLiteCommand, Task<T>> handler)
        {
            if(!ConnectionByString.TryGetValue(connectionString, out var connection))
            {
                connection = new SQLiteConnection(connectionString);
                await connection.OpenAsync();

                ConnectionByString.Add(connectionString, connection);
            }

            if(!TransactionByConnection.TryGetValue(connection, out var transaction))
            {
                transaction = (SQLiteTransaction)await connection.BeginTransactionAsync();
                TransactionByConnection.Add(connection, transaction);
            }

            using(var command = connection.CreateCommand())
            {
                command.Transaction = transaction;

                var result = await handler.Invoke(connection, command);
                return result;
            }
        }

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Test only
                    try { Console.WriteLine("Disposing..."); }
                    catch { }

                    foreach(var transaction in TransactionByConnection.Values)
                        transaction.Dispose();
                    TransactionByConnection.Clear();

                    foreach(var conn in ConnectionByString.Values)
                        conn.Dispose();
                    ConnectionByString.Clear();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}