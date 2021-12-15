using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Infrastructure.Abstractions;
using MongoDB.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure
{
    public class MongoDbContext : IMongoDbContext, IDisposable
    {
        #region Private Fields

        private readonly MongoClient _client;
        private readonly IList<object> _commands;

        private static readonly object _emptyResult = new();

        #endregion Private Fields

        #region IMongoDbContext Members

        public IMongoClient Client => _client;
        public IMongoDatabase Database { get; private set; }
        public IClientSessionHandle Session { get; private set; }
        public bool AcceptAllChangesOnSave { get; protected set; } = true;

        #endregion IMongoDbContext Members

        #region Ctor

        public MongoDbContext(MongoClientSettings clientSettings, string databaseName, MongoDatabaseSettings databaseSettings = null)
        {
            if (clientSettings == null)
            {
                throw new ArgumentNullException(nameof(clientSettings));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            _commands = new List<object>();
            _client = new MongoClient(clientSettings);
            Database = _client.GetDatabase(databaseName, databaseSettings);
        }

        public MongoDbContext(MongoUrl url, string databaseName, MongoDatabaseSettings databaseSettings = null)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            _commands = new List<object>();
            _client = new MongoClient(url);
            Database = _client.GetDatabase(databaseName, databaseSettings);
        }

        public MongoDbContext(string connectionString, string databaseName, MongoDatabaseSettings databaseSettings = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            _commands = new List<object>();
            _client = new MongoClient(connectionString);
            Database = _client.GetDatabase(databaseName, databaseSettings);
        }

        public MongoDbContext(IMongoClient client, IMongoDatabase database)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (client is not MongoClient clientImpl)
            {
                throw new InvalidCastException($"{nameof(client)} should be of type '{nameof(MongoClient)}'.");
            }

            _commands = new List<object>();
            _client = clientImpl;
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public MongoDbContext(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _commands = new List<object>();

            var clientSettings = configuration.GetSection("MongoSettings:MongoClientSettings")?.Get<MongoClientSettings>();

            if (clientSettings == null)
            {
                throw new ArgumentNullException(nameof(clientSettings));
            }

            _client = new MongoClient(clientSettings);

            var databaseName = configuration.GetValue<string>("MongoSettings:DatabaseName");
            var databaseSettings = configuration.GetSection("MongoSettings:MongoDatabaseSettings")?.Get<MongoDatabaseSettings>();

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            Database = _client.GetDatabase(databaseName, databaseSettings);
        }

        #endregion Ctor

        #region ISyncMongoDbContext Members

        public ISaveChangesResult SaveChanges()
        {
            if (!AcceptAllChangesOnSave)
            {
                return SaveChangesResult.Empty;
            }

            var saveChangesResult = new SaveChangesResult();

            try
            {
                foreach (var command in _commands)
                {
                    object commandResult = null;

                    if (command is Func<Task<object>> asyncCommand)
                    {
                        commandResult = AsyncHelper.RunSync(async () => await asyncCommand.Invoke().ConfigureAwait(continueOnCapturedContext: false));
                    }
                    else if (command is Func<object> syncCommand)
                    {
                        commandResult = syncCommand.Invoke();
                    }
                    else
                    {
                        throw new NotSupportedException("Unknown command type.");
                    }

                    saveChangesResult.Add(commandResult);
                }
            }
            finally
            {
                ClearCommands();
            }

            return saveChangesResult;
        }

        public bool HasChanges() => _commands.Any();

        public void DiscardChanges() => ClearCommands();

        public object AddCommand(Func<object> command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands.Add(command);

            return _emptyResult;
        }

        public IMongoCollection<T> GetCollection<T>(MongoCollectionSettings settings = null)
        {
            var collection = GetCollection<T>(typeof(T).Name, settings);

            return collection;
        }

        public IMongoCollection<T> GetCollection<T>(string name, MongoCollectionSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            var collection = Database.GetCollection<T>(name, settings);

            return collection;
        }

        public IClientSessionHandle StartSession(ClientSessionOptions options = null)
        {
            if (Session != null)
            {
                throw new InvalidOperationException("There's already an active session.");
            }

            return Session = _client.StartSession(options);
        }

        public void StartTransaction(ClientSessionOptions sessionOptions = null, TransactionOptions transactionOptions = null)
        {
            _ = StartSession(sessionOptions);

            Session.StartTransaction(transactionOptions);
        }

        public void CommitTransaction()
        {
            try
            {
                if (Session == null)
                {
                    throw new InvalidOperationException("There's no active session.");
                }

                Session.CommitTransaction();
            }
            catch
            {
                AbortTransaction();

                throw;
            }
            finally
            {
                DisposeSession();
            }
        }

        public void AbortTransaction()
        {
            try
            {
                if (Session != null)
                {
                    Session.AbortTransaction();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                DisposeSession();
            }
        }

        #endregion ISyncMongoDbContext Members

        #region AsyncMongoDbContext Members

        public async Task<ISaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (!AcceptAllChangesOnSave)
            {
                return SaveChangesResult.Empty;
            }

            var saveChangesResult = new SaveChangesResult();

            try
            {
                foreach (var command in _commands)
                {
                    object commandResult = null;

                    cancellationToken.ThrowIfCancellationRequested();

                    if (command is Func<Task<object>> asyncCommand)
                    {
                        commandResult = await asyncCommand.Invoke().ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else if (command is Func<object> syncCommand)
                    {
                        commandResult = syncCommand.Invoke();
                    }
                    else
                    {
                        throw new NotSupportedException("Unknown command type.");
                    }

                    saveChangesResult.Add(commandResult);
                }
            }
            finally
            {
                ClearCommands();
            }

            return saveChangesResult;
        }

        public Task<object> AddCommandAsync(Func<Task<object>> command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands.Add(command);

            return Task.FromResult(_emptyResult);
        }

        public async Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default)
        {
            if (Session != null)
            {
                throw new InvalidOperationException("There's already an active session.");
            }

            return Session = await _client.StartSessionAsync(options).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Session == null)
                {
                    throw new InvalidOperationException("There's no active session.");
                }

                await Session.CommitTransactionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch
            {
                await AbortTransactionAsync().ConfigureAwait(continueOnCapturedContext: false);

                throw;
            }
            finally
            {
                DisposeSession();
            }
        }

        public async Task AbortTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Session != null)
                {
                    await Session.AbortTransactionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                DisposeSession();
            }
        }

        #endregion AsyncMongoDbContext Members

        #region Public Methods

        public static void ApplyConfigurationsFromAssembly(Assembly targetAssembly)
        {
            if (targetAssembly == null)
            {
                throw new ArgumentNullException(nameof(targetAssembly));
            }

            var configurationType = typeof(IMongoDbFluentConfiguration);

            var configurationTypes = AssemblyScanner.Scan(
                targetAssembly,
                (Type scannedType) => configurationType.IsAssignableFrom(scannedType) && !scannedType.IsInterface && !scannedType.IsAbstract
            );

            if (configurationTypes?.Any() ?? false)
            {
                foreach (var configurationTypeItem in configurationTypes)
                {
                    var configurationInstance = Activator.CreateInstance(configurationTypeItem) as IMongoDbFluentConfiguration;

                    configurationInstance?.Configure();
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void ClearCommands() => _commands.Clear();

        private void DisposeSession()
        {
            if (Session != null)
            {
                Session.Dispose();
                Session = null;
            }
        }

        #endregion Private Methods

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DiscardChanges();
                    DisposeSession();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
