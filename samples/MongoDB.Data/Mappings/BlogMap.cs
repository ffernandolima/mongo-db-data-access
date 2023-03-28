using MongoDB.Bson.Serialization;
using MongoDB.Generators;
using MongoDB.Infrastructure;
using MongoDB.Models;

namespace MongoDB.Data.Mappings
{
    public class BlogMap : IMongoDbFluentConfiguration
    {
        public bool IsConfigured => BsonClassMap.IsClassMapRegistered(typeof(Blog));

        public void Configure()
        {
            if (IsConfigured)
            {
                return;
            }

            BsonClassMap.RegisterClassMap<Blog>(builder =>
            {
                builder.AutoMap();
                builder.MapIdMember(x => x.Id).SetIdGenerator(Int32IdGenerator<Blog>.Instance);
            });
        }
    }
}
