using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Generators;

namespace MongoDB.Tests.Stubs
{
    public class StubInt64IdEntity
    {
        [BsonId(IdGenerator = typeof(Int64IdGenerator<StubInt64IdEntity>))]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
