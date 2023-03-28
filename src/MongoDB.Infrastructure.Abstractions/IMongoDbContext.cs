using MongoDB.Driver;

namespace MongoDB.Infrastructure
{
    public interface IMongoDbContext : IMongoDbSyncContext, IMongoDbAsyncContext
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        IClientSessionHandle Session { get; }
        IMongoDbContextOptions Options { get; }
    }
}
