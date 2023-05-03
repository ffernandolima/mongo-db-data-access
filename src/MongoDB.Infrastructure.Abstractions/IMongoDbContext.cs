using MongoDB.Driver;
using System;

namespace MongoDB.Infrastructure
{
    public interface IMongoDbContext : IMongoDbSyncContext, IMongoDbAsyncContext, IDisposable
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        IClientSessionHandle Session { get; }
        IMongoDbContextOptions Options { get; }
    }

    public interface IMongoDbContext<T> : IMongoDbContext where T : IMongoDbContext
    {

    }
}
