using System;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public interface IMongoDbQueryBuilder<T, TBuilder>
        where T : class
        where TBuilder : IMongoDbQueryBuilder<T, TBuilder>
    {
        TBuilder AndFilter(Expression<Func<T, bool>> predicate);
        TBuilder OrFilter(Expression<Func<T, bool>> predicate);

        TBuilder OrderBy(Expression<Func<T, object>> keySelector);
        TBuilder ThenBy(Expression<Func<T, object>> keySelector);
        TBuilder OrderBy(string fieldName);
        TBuilder ThenBy(string fieldName);

        TBuilder OrderByDescending(Expression<Func<T, object>> keySelector);
        TBuilder ThenByDescending(Expression<Func<T, object>> keySelector);
        TBuilder OrderByDescending(string fieldName);
        TBuilder ThenByDescending(string fieldName);

        TBuilder Select(Expression<Func<T, T>> selector);
    }

    public interface IMongoDbQueryBuilder<T, TResult, TBuilder>
        where T : class
        where TBuilder : IMongoDbQueryBuilder<T, TResult, TBuilder>
    {
        TBuilder AndFilter(Expression<Func<T, bool>> predicate);
        TBuilder OrFilter(Expression<Func<T, bool>> predicate);

        TBuilder OrderBy(Expression<Func<T, object>> keySelector);
        TBuilder ThenBy(Expression<Func<T, object>> keySelector);
        TBuilder OrderBy(string fieldName);
        TBuilder ThenBy(string fieldName);

        TBuilder OrderByDescending(Expression<Func<T, object>> keySelector);
        TBuilder ThenByDescending(Expression<Func<T, object>> keySelector);
        TBuilder OrderByDescending(string fieldName);
        TBuilder ThenByDescending(string fieldName);

        TBuilder Select(Expression<Func<T, TResult>> selector);
    }
}
