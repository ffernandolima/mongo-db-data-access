using MongoDB.Driver;
using MongoDB.Infrastructure;

namespace MongoDB.Data
{
    public class BloggingContext : MongoDbContext
    {
        public BloggingContext(IMongoClient client, IMongoDatabase database, IMongoDbContextOptions options)
            : base(client, database, options)
        { }
    }
}
