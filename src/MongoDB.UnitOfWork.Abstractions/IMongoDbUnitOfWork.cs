using MongoDB.Infrastructure;
using System;

namespace MongoDB.UnitOfWork
{
    public interface IMongoDbUnitOfWork : IMongoDbSyncUnitOfWork, IMongoDbAsyncUnitOfWork, IDisposable
    {
        IMongoDbContext Context { get; }
    }

    public interface IMongoDbUnitOfWork<T> : IMongoDbUnitOfWork, ISyncMongoDbUnitOfWork<T>, IAsyncMongoDbUnitOfWork<T>, IDisposable where T : IMongoDbContext
    { }
}
