using MongoDB.Driver;
using MongoDB.Infrastructure;

namespace MongoDB.Data
{
    public class PollContext : MongoDbContext, IMongoDbContext<PollContext>
    {
        public PollContext(IMongoClient client, IMongoDatabase database, IMongoDbContextOptions options)
            : base(client, database, options)
        { }
    }
}
