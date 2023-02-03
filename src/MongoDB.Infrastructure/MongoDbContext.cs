using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure
{
    public class MongoDbContext : IMongoDbContext
    {
        #region Private Fields

        private readonly MongoClient _client;
        private readonly AsyncLocal<IList<object>> _commands;
        private readonly AsyncLocal<MongoDbSession> _session;
        private readonly IThrottlingMongoDbSemaphore _semaphore;
        private readonly int _maximumNumberOfConcurrentRequests;

        private static readonly object _sync = new();
        private static readonly object _emptyResult = new();

        #endregion Private Fields

        #region IMongoDbContext Members

        public IMongoClient Client => _client;
        public IMongoDatabase Database { get; private set; }
        public IClientSessionHandle Session => GetSession();
        public bool AcceptAllChangesOnSave { get; protected set; } = true;
        public int MaximumNumberOfConcurrentRequests
        {
            get => _maximumNumberOfConcurrentRequests;
            protected init
            {
                _maximumNumberOfConcurrentRequests = value;
                if (_maximumNumberOfConcurrentRequests == 0)
                {
                    _maximumNumberOfConcurrentRequests = GetDefaultMaximumNumberOfConcurrentRequests();
                }

                _semaphore = _maximumNumberOfConcurrentRequests > 0
                    ? new ThrottlingMongoDbSemaphore(_maximumNumberOfConcurrentRequests)
                    : NoopThrottlingMongoDbSemaphore.Instance;
            }
        }

        #endregion IMongoDbContext Members

        #region Ctor

        public MongoDbContext(MongoClientSettings clientSettings, string databaseName, MongoDatabaseSettings databaseSettings = null)
        {
            if (clientSettings is null)
            {
                throw new ArgumentNullException(nameof(clientSettings));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            clientSettings.AddDiagnostics();

            _commands = new AsyncLocal<IList<object>>();
            _session = new AsyncLocal<MongoDbSession>();
            _client = new MongoClient(clientSettings);
            Database = _client.GetDatabase(databaseName, databaseSettings);
            MaximumNumberOfConcurrentRequests = GetDefaultMaximumNumberOfConcurrentRequests();
        }

        public MongoDbContext(MongoUrl url, string databaseName, MongoDatabaseSettings databaseSettings = null)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            var clientSettings = MongoClientSettings.FromUrl(url).AddDiagnostics();

            _commands = new AsyncLocal<IList<object>>();
            _session = new AsyncLocal<MongoDbSession>();
            _client = new MongoClient(clientSettings);
            Database = _client.GetDatabase(databaseName, databaseSettings);
            MaximumNumberOfConcurrentRequests = GetDefaultMaximumNumberOfConcurrentRequests();
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

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString).AddDiagnostics();

            _commands = new AsyncLocal<IList<object>>();
            _session = new AsyncLocal<MongoDbSession>();
            _client = new MongoClient(clientSettings);
            Database = _client.GetDatabase(databaseName, databaseSettings);
            MaximumNumberOfConcurrentRequests = GetDefaultMaximumNumberOfConcurrentRequests();
        }

        public MongoDbContext(IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _commands = new AsyncLocal<IList<object>>();
            _session = new AsyncLocal<MongoDbSession>();

            var clientSettings = configuration.GetSection("MongoSettings:MongoClientSettings")?.Get<MongoClientSettings>();

            if (clientSettings is null)
            {
                throw new ArgumentNullException(nameof(clientSettings));
            }

            clientSettings.AddDiagnostics();

            _client = new MongoClient(clientSettings);

            var databaseName = configuration.GetValue<string>("MongoSettings:DatabaseName");
            var databaseSettings = configuration.GetSection("MongoSettings:MongoDatabaseSettings")?.Get<MongoDatabaseSettings>();

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            Database = _client.GetDatabase(databaseName, databaseSettings);
            MaximumNumberOfConcurrentRequests = GetDefaultMaximumNumberOfConcurrentRequests();
        }

        #endregion Ctor

        #region ISyncMongoDbContext Members

        public ISaveChangesResult SaveChanges()
        {
            if (!AcceptAllChangesOnSave || !(_commands.Value?.Any() ?? false))
            {
                return SaveChangesResult.Empty;
            }

            var saveChangesResult = new SaveChangesResult();

            try
            {
                foreach (var command in _commands.Value)
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

        public bool HasChanges() => _commands.Value?.Any() ?? false;

        public void DiscardChanges() => ClearCommands();

        public object AddCommand(Func<object> command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands.Value ??= new List<object>();
            _commands.Value.Add(command);

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

            var collection = new ThrottlingMongoDbCollection<T>(_semaphore, Database.GetCollection<T>(name, settings));

            return collection;
        }

        public IClientSessionHandle StartSession(ClientSessionOptions options = null)
        {
            var session = GetSession() ?? CreateSession(options);

            return session;
        }

        public void StartTransaction(ClientSessionOptions sessionOptions = null, TransactionOptions transactionOptions = null)
        {
            var session = StartSession(sessionOptions);

            session.StartTransaction(transactionOptions);
        }

        public void CommitTransaction()
        {
            try
            {
                var session = GetSession();

                if (session is null)
                {
                    throw new InvalidOperationException("There's no active session.");
                }

                session.CommitTransaction();
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
                var session = GetSession();

                session?.AbortTransaction();
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
            if (!AcceptAllChangesOnSave || !(_commands.Value?.Any() ?? false))
            {
                return SaveChangesResult.Empty;
            }

            var saveChangesResult = new SaveChangesResult();

            try
            {
                foreach (var command in _commands.Value)
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
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands.Value ??= new List<object>();
            _commands.Value.Add(command);

            return Task.FromResult(_emptyResult);
        }

        public async Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default)
        {
            var session = GetSession() ?? await CreateSessionAsync(options).ConfigureAwait(continueOnCapturedContext: false);

            return session;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var session = GetSession();

                if (session is null)
                {
                    throw new InvalidOperationException("There's no active session.");
                }

                await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
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
                var session = GetSession();

                if (session is not null)
                {
                    await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
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
            if (targetAssembly is null)
            {
                throw new ArgumentNullException(nameof(targetAssembly));
            }

            lock (_sync)
            {
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
        }

        #endregion Public Methods

        #region Private Methods

        private int GetDefaultMaximumNumberOfConcurrentRequests() => Math.Max(_client.Settings.MaxConnectionPoolSize / 2, 1);

        private void ClearCommands() => _commands.Value?.Clear();

        private IClientSessionHandle GetSession()
        {
            var session = _session.Value?.SessionHandle;

            return session;
        }

        private IClientSessionHandle CreateSession(ClientSessionOptions options = null)
        {
            var session = _client.StartSession(options);

            _session.Value = new MongoDbSession(session);

            return session;
        }

        public async Task<IClientSessionHandle> CreateSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default)
        {
            var session = await _client.StartSessionAsync(options, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

            _session.Value = new MongoDbSession(session);

            return session;
        }

        private void DisposeSession()
        {
            _session.Value?.SessionHandle.Dispose();
            _session.Value = null;
        }

        #endregion Private Methods
    }
}
