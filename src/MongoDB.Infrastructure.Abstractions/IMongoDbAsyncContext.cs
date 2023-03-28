using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure
{
    public interface IMongoDbAsyncContext
    {
        Task<IMongoDbSaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<object> AddCommandAsync(Func<Task<object>> command);
        Task<IClientSessionHandle> StartSessionAsync(
            ClientSessionOptions options = null,
            CancellationToken cancellationToken = default);
        Task StartTransactionAsync(
            ClientSessionOptions sessionOptions = null,
            TransactionOptions transactionOptions = null,
            CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task AbortTransactionAsync(CancellationToken cancellationToken = default);
    }
}
