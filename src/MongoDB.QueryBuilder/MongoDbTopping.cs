namespace MongoDB.QueryBuilder
{
    public class MongoDbTopping : IMongoDbTopping
    {
        internal MongoDbTopping()
        { }

        public int? TopRows { get; internal set; }
        public bool IsEnabled => TopRows > 0;
    }
}
