using MongoDB.Bson.Serialization;
using MongoDB.Generators;
using MongoDB.Infrastructure;
using MongoDB.Models;

namespace MongoDB.Data.Mappings
{
    public class BlogMap : IMongoDbFluentConfiguration
    {
        public void Configure()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Blog)))
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
