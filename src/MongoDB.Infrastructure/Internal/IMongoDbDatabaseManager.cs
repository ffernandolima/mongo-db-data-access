using MongoDB.Driver;

namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbDatabaseManager
    {
        IMongoDatabase GetOrCreate(IMongoClient client, string databaseName, MongoDatabaseSettings databaseSettings = null);
    }
}
