using MongoDB.Driver;
using System;

namespace MongoDB.Infrastructure.Abstractions
{
    public interface ISyncMongoDbContext : IDisposable
    {
        ISaveChangesResult SaveChanges();
        bool HasChanges();
        void DiscardChanges();
        object AddCommand(Func<object> command);
        IMongoCollection<T> GetCollection<T>(MongoCollectionSettings settings = null);
        IMongoCollection<T> GetCollection<T>(string name, MongoCollectionSettings settings = null);
        IClientSessionHandle StartSession(ClientSessionOptions options = null);
        void StartTransaction(ClientSessionOptions sessionOptions = null, TransactionOptions transactionOptions = null);
        void CommitTransaction();
        void AbortTransaction();
    }
}
