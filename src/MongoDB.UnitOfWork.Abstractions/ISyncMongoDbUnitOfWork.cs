using MongoDB.Driver;
using MongoDB.Infrastructure;
using System;

namespace MongoDB.UnitOfWork
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
