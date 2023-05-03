using MongoDB.Driver;
using MongoDB.Infrastructure;

namespace MongoDB.Data
{
    public class BloggingContext : MongoDbContext, IMongoDbContext<BloggingContext>
    {
        public BloggingContext(IMongoClient client, IMongoDatabase database, IMongoDbContextOptions options)
            : base(client, database, options)
        { }
    }
}
