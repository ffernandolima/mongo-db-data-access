using System.Collections.Generic;

namespace MongoDB.Infrastructure
{
    public sealed class SaveChangesResult : ISaveChangesResult
    {
        private readonly List<object> _results;

        internal static readonly SaveChangesResult Empty = new();

        public IReadOnlyList<object> Results => _results.AsReadOnly();

        internal SaveChangesResult() => _results = new List<object>();

        internal void Add(object result) => _results.Add(result);
    }
}
