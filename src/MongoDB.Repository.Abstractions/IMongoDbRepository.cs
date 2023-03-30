using MongoDB.Driver.Linq;
using MongoDB.QueryBuilder;
using System;

namespace MongoDB.Repository
{
    public interface IMongoDbRepository : IDisposable
    { }

    public interface IMongoDbRepository<T> : IMongoDbRepository, ISyncMongoDbRepository<T>, IAsyncMongoDbRepository<T>, IDisposable where T : class
    {
        IMongoQueryable<T> ToQueryable(IMongoDbQuery<T> query);
        IMongoQueryable<TResult> ToQueryable<TResult>(IMongoDbQuery<T, TResult> query);
    }
}
