using System;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public interface IMongoDbMultipleResultQuery
    {
        IMongoDbPaging Paging { get; }
        IMongoDbTopping Topping { get; }
    }

    public interface IMongoDbMultipleResultQuery<T> : IMongoDbMultipleResultQuery, IMongoDbQuery<T>, IMongoDbQueryBuilder<T, IMongoDbMultipleResultQuery<T>> where T : class
    {
        IMongoDbMultipleResultQuery<T> Page(int? pageIndex, int? pageSize);
        IMongoDbMultipleResultQuery<T> Top(int? topRows);
        IMongoDbMultipleResultQuery<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector);
    }

    public interface IMongoDbMultipleResultQuery<T, TResult> : IMongoDbMultipleResultQuery, IMongoDbQuery<T, TResult>, IMongoDbQueryBuilder<T, TResult, IMongoDbMultipleResultQuery<T, TResult>> where T : class
    {
        IMongoDbMultipleResultQuery<T, TResult> Page(int? pageIndex, int? pageSize);
        IMongoDbMultipleResultQuery<T, TResult> Top(int? topRows);
    }
}
