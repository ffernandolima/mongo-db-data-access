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

        IMongoDbQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        IMongoDbQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);
        IMongoDbQuery<T> OrderBy(string fieldName);
        IMongoDbQuery<T> ThenBy(string fieldName);

        IMongoDbQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        IMongoDbQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
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

        IMongoDbQuery<T, TResult> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        IMongoDbQuery<T, TResult> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);
        IMongoDbQuery<T, TResult> OrderBy(string fieldName);
        IMongoDbQuery<T, TResult> ThenBy(string fieldName);

        IMongoDbQuery<T, TResult> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        IMongoDbQuery<T, TResult> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        IMongoDbQuery<T, TResult> OrderByDescending(string fieldName);
        IMongoDbQuery<T, TResult> ThenByDescending(string fieldName);

        IMongoDbQuery<T, TResult> Select(Expression<Func<T, TResult>> selector);
    }
}
