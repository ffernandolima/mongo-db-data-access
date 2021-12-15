using MongoDB.QueryBuilder.Abstractions;

namespace MongoDB.QueryBuilder
{
    public class MongoDbPaging : IMongoDbPaging
    {
        internal MongoDbPaging()
        { }

        public int? PageIndex { get; internal set; }
        public int? PageSize { get; internal set; }
        public int TotalCount { get; internal set; }
        public bool IsEnabled => PageSize > 0;
    }
}
