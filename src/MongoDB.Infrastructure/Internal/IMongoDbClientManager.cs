using MongoDB.Driver;

namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbClientManager
    {
        IMongoClient GetOrCreate(MongoClientSettings clientSettings);
    }
}
