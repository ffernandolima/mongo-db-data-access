using MongoDB.Infrastructure.Abstractions;
using System;

namespace MongoDB.UnitOfWork.Abstractions
{
    public interface IMongoDbUnitOfWork : ISyncMongoDbUnitOfWork, IAsyncMongoDbUnitOfWork, IDisposable
    {
        IMongoDbContext Context { get; }
    }

    public interface IMongoDbUnitOfWork<T> : IMongoDbUnitOfWork, ISyncMongoDbUnitOfWork<T>, IAsyncMongoDbUnitOfWork<T>, IDisposable where T : IMongoDbContext
    { }
}
