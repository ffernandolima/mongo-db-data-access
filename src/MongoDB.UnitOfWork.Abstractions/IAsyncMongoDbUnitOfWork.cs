using MongoDB.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.UnitOfWork
{
    public interface IAsyncMongoDbUnitOfWork : IMongoDbRepositoryFactory, IDisposable
    {
        Task<ISaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task AbortTransactionAsync(CancellationToken cancellationToken = default);
    }

    public interface IAsyncMongoDbUnitOfWork<T> : IAsyncMongoDbUnitOfWork, IMongoDbRepositoryFactory<T>, IDisposable where T : IMongoDbContext
    { }
}
