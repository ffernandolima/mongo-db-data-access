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

        IMongoDbQuery<T> AndFilter(Expression<Func<T, bool>> predicate);
        IMongoDbQuery<T> OrFilter(Expression<Func<T, bool>> predicate);

        IMongoDbQuery<T> OrderBy(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T> ThenBy(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T> OrderBy(string fieldName);
        IMongoDbQuery<T> ThenBy(string fieldName);

        IMongoDbQuery<T> OrderByDescending(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T> ThenByDescending(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T> OrderByDescending(string fieldName);
        IMongoDbQuery<T> ThenByDescending(string fieldName);

        IMongoDbQuery<T> Select(Expression<Func<T, T>> selector);
        IMongoDbQuery<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector);
    }

    public interface IMongoDbQuery<T, TResult> : IMongoDbQuery where T : class
    {
        Expression<Func<T, bool>> Predicate { get; }
        Expression<Func<T, TResult>> Selector { get; }
        IList<IMongoDbSorting<T>> Sortings { get; }

        IMongoDbQuery<T, TResult> AndFilter(Expression<Func<T, bool>> predicate);
        IMongoDbQuery<T, TResult> OrFilter(Expression<Func<T, bool>> predicate);

        IMongoDbQuery<T, TResult> OrderBy(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T, TResult> ThenBy(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T, TResult> OrderBy(string fieldName);
        IMongoDbQuery<T, TResult> ThenBy(string fieldName);

        IMongoDbQuery<T, TResult> OrderByDescending(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T, TResult> ThenByDescending(Expression<Func<T, object>> keySelector);
        IMongoDbQuery<T, TResult> OrderByDescending(string fieldName);
        IMongoDbQuery<T, TResult> ThenByDescending(string fieldName);

        IMongoDbQuery<T, TResult> Select(Expression<Func<T, TResult>> selector);
    }
}
