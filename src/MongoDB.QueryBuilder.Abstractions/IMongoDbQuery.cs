using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public interface IMongoDbQuery
    { }

    public interface IMongoDbQuery<T> : IMongoDbQuery where T : class
    {
        Expression<Func<T, bool>> Predicate { get; }
        IList<IMongoDbSorting<T>> Sortings { get; }
        Expression<Func<T, T>> Selector { get; }
    }

    public interface IMongoDbQuery<T, TResult> : IMongoDbQuery where T : class
    {
        Expression<Func<T, bool>> Predicate { get; }
        IList<IMongoDbSorting<T>> Sortings { get; }
        Expression<Func<T, TResult>> Selector { get; }
    }
}
