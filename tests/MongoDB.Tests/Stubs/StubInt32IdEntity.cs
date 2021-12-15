using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Generators;

namespace MongoDB.Tests.Stubs
{
    public class StubInt32IdEntity
    {
        [BsonId(IdGenerator = typeof(Int32IdGenerator<StubInt32IdEntity>))]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
