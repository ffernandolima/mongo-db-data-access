using MongoDB.QueryBuilder.Abstractions;
using System;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder.Internal
{
    internal static class MongoDbQueryExtensions
    {
        public static IMongoDbQuery<T, TResult> ToQuery<T, TResult>(this IMongoDbQuery<T> sourceQuery, Expression<Func<T, TResult>> selector = null) where T : class
        {
            MongoDbQuery<T, TResult> destinationQuery = null;

            if (sourceQuery is IMongoDbSingleResultQuery<T>)
            {
                destinationQuery = new MongoDbSingleResultQuery<T, TResult>();
            }

            if (sourceQuery is IMongoDbMultipleResultQuery<T> multipleResultQuery)
            {
                destinationQuery = new MongoDbMultipleResultQuery<T, TResult>
                {
                    Paging = multipleResultQuery.Paging,
                    Topping = multipleResultQuery.Topping
                };
            }

            if (destinationQuery != null)
            {
                destinationQuery.Predicate = sourceQuery.Predicate;
                destinationQuery.Sortings = sourceQuery.Sortings;

                if (selector != null)
                {
                    destinationQuery.Selector = selector;
                }
            }

            return destinationQuery;
        }
    }
}
