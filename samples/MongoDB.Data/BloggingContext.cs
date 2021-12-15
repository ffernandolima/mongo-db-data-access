using MongoDB.Driver;
using MongoDB.Infrastructure;

namespace MongoDB.Data
{
    public class BloggingContext : MongoDbContext
    {
        public BloggingContext(string connectionString, string databaseName, MongoDatabaseSettings databaseSettings = null)
            : base(connectionString, databaseName, databaseSettings)
        {
            AcceptAllChangesOnSave = true;
            ApplyConfigurationsFromAssembly(typeof(BloggingContext).Assembly);
        }
    }
}
