using System;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public interface IMongoDbSingleResultQuery<T> : IMongoDbQuery<T>, IMongoDbQueryBuilder<T, IMongoDbSingleResultQuery<T>> where T : class
    {
        IMongoDbSingleResultQuery<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector);
    }

    public interface IMongoDbSingleResultQuery<T, TResult> : IMongoDbQuery<T, TResult>, IMongoDbQueryBuilder<T, TResult, IMongoDbSingleResultQuery<T, TResult>> where T : class
    { }
}
