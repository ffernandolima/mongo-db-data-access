using MongoDB.Driver;
using MongoDB.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.UnitOfWork
{
    public interface IMongoDbAsyncUnitOfWork : IMongoDbRepositoryFactory, IDisposable
    {
        Task<IMongoDbSaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task StartTransactionAsync(
            ClientSessionOptions sessionOptions = null,
            TransactionOptions transactionOptions = null,
            CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task AbortTransactionAsync(CancellationToken cancellationToken = default);
    }

    public interface IAsyncMongoDbUnitOfWork<T> : IMongoDbAsyncUnitOfWork, IMongoDbRepositoryFactory<T>, IDisposable where T : IMongoDbContext
    { }
}
