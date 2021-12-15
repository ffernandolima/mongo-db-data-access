using MongoDB.Driver;
using MongoDB.Infrastructure.Abstractions;
using System;

namespace MongoDB.UnitOfWork.Abstractions
{
    public interface ISyncMongoDbUnitOfWork : IMongoDbRepositoryFactory, IDisposable
    {
        ISaveChangesResult SaveChanges();
        bool HasChanges();
        void DiscardChanges();
        void StartTransaction(ClientSessionOptions sessionOptions = null, TransactionOptions transactionOptions = null);
        void CommitTransaction();
        void AbortTransaction();
    }

    public interface ISyncMongoDbUnitOfWork<T> : ISyncMongoDbUnitOfWork, IMongoDbRepositoryFactory<T>, IDisposable where T : IMongoDbContext
    { }
}
