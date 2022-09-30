using MongoDB.Driver;
using MongoDB.Infrastructure;
using MongoDB.Repository;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.UnitOfWork
{
    public class MongoDbUnitOfWork : IMongoDbUnitOfWork, IDisposable
    {
        #region Private Fields

        private readonly IMongoDbServiceFactory _factory;
        private readonly ConcurrentDictionary<string, IMongoDbRepository> _repositories;

        #endregion Private Fields

        #region IMongoDbUnitOfWork Members

        public IMongoDbContext Context { get; }

        #endregion IMongoDbUnitOfWork Members

        #region Ctor

        public MongoDbUnitOfWork(IMongoDbContext context, IMongoDbServiceFactory factory)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _repositories = new ConcurrentDictionary<string, IMongoDbRepository>();
        }

        #endregion Ctor

        #region IRepositoryFactory Members

        /// <typeparam name="T">Custom repository interface</typeparam>
        public T CustomRepository<T>() where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Generic type should be an interface.");
            }

            static IMongoDbRepository Factory(IMongoDbContext dbContext, Type type)
            {
                return (IMongoDbRepository)AppDomain.CurrentDomain.GetAssemblies()
                                                    .SelectMany(selector => selector.GetTypes())
                                                    .Where(predicate => type.IsAssignableFrom(predicate) && !predicate.IsInterface && !predicate.IsAbstract)
                                                    .Select(selector => Activator.CreateInstance(selector, dbContext))
                                                    .SingleOrDefault();
            }

            return _factory.GetService<T>() ?? (T)GetRepository(typeof(T), Factory, "Custom");
        }

        public IMongoDbRepository<T> Repository<T>() where T : class
        {
            static IMongoDbRepository Factory(IMongoDbContext dbContext, Type type) => new MongoDbRepository<T>(dbContext);

            return _factory.GetService<IMongoDbRepository<T>>() ?? (IMongoDbRepository<T>)GetRepository(typeof(T), Factory, "Generic");
        }

        #endregion IRepositoryFactory Members

        #region ISyncMongoDbUnitOfWork Members

        public ISaveChangesResult SaveChanges() => Context.SaveChanges();

        public bool HasChanges() => Context.HasChanges();

        public void DiscardChanges() => Context.DiscardChanges();

        public void StartTransaction(ClientSessionOptions sessionOptions = null, TransactionOptions transactionOptions = null) => Context.StartTransaction(sessionOptions, transactionOptions);

        public void CommitTransaction() => Context.CommitTransaction();

        public void AbortTransaction() => Context.AbortTransaction();

        #endregion ISyncMongoDbUnitOfWork Members

        #region IAsyncMongoDbUnitOfWork Members

        public Task<ISaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default) => Context.SaveChangesAsync(cancellationToken);

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Context.CommitTransactionAsync(cancellationToken);

        public Task AbortTransactionAsync(CancellationToken cancellationToken = default) => Context.AbortTransactionAsync(cancellationToken);

        #endregion IAsyncMongoDbUnitOfWork Members

        #region Private Methods

        private IMongoDbRepository GetRepository(Type objectType, Func<IMongoDbContext, Type, IMongoDbRepository> repositoryFactory, string prefix)
        {
            var typeName = $"{prefix}.{objectType.FullName}";

            if (!_repositories.TryGetValue(typeName, out var repository))
            {
                repository = repositoryFactory.Invoke(Context, objectType);

                _repositories[typeName] = repository;
            }

            return repository;
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
                    foreach (var repository in _repositories.Values)
                    {
                        repository.Dispose();
                    }

                    _repositories.Clear();

                    _factory.Dispose();
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

    public class MongoDbUnitOfWork<T> : MongoDbUnitOfWork, IMongoDbUnitOfWork<T> where T : IMongoDbContext
    {
        #region Ctor

        public MongoDbUnitOfWork(T context, IMongoDbServiceFactory factory)
            : base(context, factory)
        { }

        #endregion Ctor
    }
}
