using MongoDB.Driver;

namespace MongoDB.Infrastructure
{
    public interface IMongoDbContext : ISyncMongoDbContext, IAsyncMongoDbContext
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        IClientSessionHandle Session { get; }
        bool AcceptAllChangesOnSave { get; }
    }
}
