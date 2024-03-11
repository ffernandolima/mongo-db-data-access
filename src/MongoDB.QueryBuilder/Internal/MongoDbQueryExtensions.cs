using System;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder.Internal
{
    internal static class MongoDbQueryExtensions
    {
        public static IMongoDbSingleResultQuery<T, TResult> ToQuery<T, TResult>(
            this IMongoDbSingleResultQuery<T> sourceQuery,
            Expression<Func<T, TResult>> selector = null)
                where T : class
        {
            var destinationQuery = new MongoDbSingleResultQuery<T, TResult>
            {
                Predicate = sourceQuery.Predicate,
                Sortings = sourceQuery.Sortings,
                Selector = selector
            };

            return destinationQuery;
        }

        public static IMongoDbMultipleResultQuery<T, TResult> ToQuery<T, TResult>(
            this IMongoDbMultipleResultQuery<T> sourceQuery,
            Expression<Func<T, TResult>> selector = null)
                where T : class
        {
            var destinationQuery = new MongoDbMultipleResultQuery<T, TResult>
            {
                Predicate = sourceQuery.Predicate,
                Sortings = sourceQuery.Sortings,
                Paging = sourceQuery.Paging,
                Topping = sourceQuery.Topping,
                Selector = selector
            };

            return destinationQuery;
        }
    }
}
