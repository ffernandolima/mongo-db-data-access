using MongoDB.Driver;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbConnection
    {
        public IMongoClient Client { get; internal set; }
        public IMongoDatabase Database { get; internal set; }
        public IMongoDbContextOptions Options { get; internal set; }
        public IMongoDbThrottlingSemaphore Semaphore { get; internal set; }
    }
}
