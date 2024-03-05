using System.Collections.Generic;

namespace MongoDB.Infrastructure
{
    public sealed class MongoDbSaveChangesResult : IMongoDbSaveChangesResult
    {
        private readonly List<object> _results;

        internal static readonly MongoDbSaveChangesResult Empty = new();

        public IReadOnlyList<object> Results => _results.AsReadOnly();

        internal MongoDbSaveChangesResult()
        {
            _results = new List<object>();
        }

        internal void Add(object result) => _results.Add(result);
    }
}
