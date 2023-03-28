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

        private static readonly object _emptyResult = new();

        private readonly IList<object> _commands;
        private readonly IMongoDbThrottlingSemaphore _semaphore;

        #endregion Private Fields

        #region IMongoDbContext Members

        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public IClientSessionHandle Session { get; private set; }
        public IMongoDbContextOptions Options { get; private set; }

        #endregion IMongoDbContext Members

        #region Ctor

        public MongoDbContext(IMongoClient client, IMongoDatabase database, IMongoDbContextOptions options)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client), $"{nameof(client)} cannot be null.");
            Database = database ?? throw new ArgumentNullException(nameof(database), $"{nameof(database)} cannot be null.");
            Options = options ?? throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");

            _commands = new List<object>();
            _semaphore = MongoDbThrottlingSemaphoreFactory.Instance.GetOrCreate(options.MaximumNumberOfConcurrentRequests);
        }

        #endregion Ctor

        #region ISyncMongoDbContext Members

        public IMongoDbSaveChangesResult SaveChanges()
        {
            if (!Options.AcceptAllChangesOnSave || !(_commands?.Any() ?? false))
            {
                return MongoDbSaveChangesResult.Empty;
            }

            var saveChangesResult = new MongoDbSaveChangesResult();

            try
            {
                foreach (var command in _commands)
                {
                    object commandResult = null;

                    if (command is Func<Task<object>> asyncCommand)
                    {
                        commandResult = MongoDbAsyncHelper.RunSync(asyncCommand.Invoke);
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

        public bool HasChanges() => _commands?.Any() ?? false;

        public void DiscardChanges() => ClearCommands();

        public object AddCommand(Func<object> command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command), $"{nameof(command)} cannot be null.");
            }

            _commands.Add(command);

            return _emptyResult;
        }

        public IMongoCollection<T> GetCollection<T>(MongoCollectionSettings settings = null)
            => GetCollection<T>(typeof(T).Name, settings);

        public IMongoCollection<T> GetCollection<T>(string name, MongoCollectionSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));
            }

            var collection = new MongoDbThrottlingCollection<T>(_semaphore, Database.GetCollection<T>(name, settings));

            return collection;
        }

        public IClientSessionHandle StartSession(ClientSessionOptions options = null)
            => Session ??= Client.StartSession(options);

        public void StartTransaction(
            ClientSessionOptions sessionOptions = null,
            TransactionOptions transactionOptions = null)
        {
            StartSession(sessionOptions);

            Session.StartTransaction(transactionOptions);
        }

        public void CommitTransaction()
        {
            try
            {
                if (Session is null)
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
                Session?.AbortTransaction();
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

        public async Task<IMongoDbSaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (!Options.AcceptAllChangesOnSave || !(_commands?.Any() ?? false))
            {
                return MongoDbSaveChangesResult.Empty;
            }

            var saveChangesResult = new MongoDbSaveChangesResult();

            try
            {
                foreach (var command in _commands)
                {
                    object commandResult = null;

                    cancellationToken.ThrowIfCancellationRequested();

                    if (command is Func<Task<object>> asyncCommand)
                    {
                        commandResult = await asyncCommand.Invoke()
                            .ConfigureAwait(continueOnCapturedContext: false);
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
                throw new ArgumentNullException(nameof(command), $"{nameof(command)} cannot be null.");
            }

            _commands.Add(command);

            return Task.FromResult(_emptyResult);
        }

        public async Task<IClientSessionHandle> StartSessionAsync(
            ClientSessionOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return Session ??= await Client.StartSessionAsync(options, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task StartTransactionAsync(
            ClientSessionOptions sessionOptions = null,
            TransactionOptions transactionOptions = null,
            CancellationToken cancellationToken = default)
        {
            await StartSessionAsync(sessionOptions, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);

            Session.StartTransaction(transactionOptions);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (Session is null)
                {
                    throw new InvalidOperationException("There's no active session.");
                }

                await Session.CommitTransactionAsync(cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
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
                if (Session is not null)
                {
                    await Session.AbortTransactionAsync(cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);
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

        public void ApplyConfigurationsFromAssembly(
            Assembly scanningAssembly,
            Func<Type, bool> scanningFilter = null)
        {
            MongoDbFluentConfigurator.ApplyConfigurationsFromAssemblies(new[] { scanningAssembly }, scanningFilter);
        }

        public void ApplyConfigurationsFromAssemblies(
            IEnumerable<Assembly> scanningAssemblies,
            Func<Type, bool> scanningFilter = null)
        {
            MongoDbFluentConfigurator.ApplyConfigurationsFromAssemblies(scanningAssemblies, scanningFilter);
        }

        #endregion Public Methods

        #region Private Methods

        private void ClearCommands() => _commands?.Clear();

        private void DisposeSession()
        {
            Session?.Dispose();
            Session = null;
        }

        #endregion Private Methods
    }
}
