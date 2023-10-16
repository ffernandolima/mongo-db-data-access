using MongoDB.Driver;
using MongoDB.Infrastructure;

namespace MongoDB.Tests.Dummies
{
    public class TestingContext : MongoDbContext
    {
        public TestingContext(IMongoClient client, IMongoDatabase database, IMongoDbContextOptions options)
            : base(client, database, options)
        { }
    }
}
