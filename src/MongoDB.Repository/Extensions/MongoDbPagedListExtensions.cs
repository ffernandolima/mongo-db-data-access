using MongoDB.Repository.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Repository.Extensions
{
    public static class MongoDbPagedListExtensions
    {
        public static IMongoDbPagedList<T> ToPagedList<T>(this IList<T> source, int? pageIndex, int? pageSize, int totalCount)
            => new MongoDbPagedList<T>(source, pageIndex, pageSize, totalCount);

        public static Task<IMongoDbPagedList<T>> ToPagedListAsync<T>(this Task<IList<T>> source, int? pageIndex, int? pageSize, int totalCount, CancellationToken cancellationToken = default)
            => source.Then<IList<T>, IMongoDbPagedList<T>>(result => new MongoDbPagedList<T>(result, pageIndex, pageSize, totalCount), cancellationToken);
    }
}
