using MongoDB.Driver;

namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbConnectionManager
    {
        MongoDbConnection GetOrCreate(
            MongoClientSettings clientSettings,
            string databaseName,
            MongoDatabaseSettings databaseSettings,
            MongoDbContextOptions dbContextOptions);
    }
}
