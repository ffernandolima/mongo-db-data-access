using System.Collections.Generic;

namespace MongoDB.Repository
{
    public interface IMongoDbPagedList<T>
    {
        int? PageIndex { get; }
        int? PageSize { get; }
        int Count { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        IList<T> Items { get; }
    }
}
