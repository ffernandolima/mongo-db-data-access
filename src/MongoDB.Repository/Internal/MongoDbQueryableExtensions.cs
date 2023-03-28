using MongoDB.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MongoDB.Repository.Internal
{
    internal static class MongoDbQueryableExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate is null)
            {
                return source;
            }

            return source.Where(predicate);
        }

        public static IQueryable<T> Top<T>(this IQueryable<T> source, IMongoDbTopping topping)
            where T : class
        {
            if (!(topping?.TopRows > 0))
            {
                return source;
            }

            return source.Take(topping.TopRows.Value);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> source, IMongoDbPaging paging)
            where T : class
        {
            if (!(paging?.PageSize > 0))
            {
                return source;
            }

            var skipCount = ((paging.PageIndex ?? 1) - 1) * paging.PageSize.Value;

            return skipCount < 0 ? source : source.Skip(skipCount).Take(paging.PageSize.Value);
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, IList<IMongoDbSorting<T>> sortings)
            where T : class
        {
            if (!(sortings?.Any() ?? false))
            {
                return source;
            }

            var orderedQueryable = false;

            foreach (var sorting in sortings.Where(s => s is not null))
            {
                if (sorting.SortingDirection == MongoDbSortingDirection.Ascending)
                {
                    if (!orderedQueryable)
                    {
                        if (!string.IsNullOrWhiteSpace(sorting.FieldName))
                        {
                            source = source.OrderBy(sorting.FieldName, out var success);

                            if (success)
                            {
                                orderedQueryable = true;
                            }
                        }
                        else if (sorting.KeySelector is not null)
                        {
                            source = sorting.KeySelector.Invoke(source);

                            orderedQueryable = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(sorting.FieldName))
                        {
                            source = ((IOrderedQueryable<T>)source).ThenBy(sorting.FieldName, out _);
                        }
                        else if (sorting.KeySelector is not null)
                        {
                            source = sorting.KeySelector.Invoke(source);
                        }
                    }
                }
                else if (sorting.SortingDirection == MongoDbSortingDirection.Descending)
                {
                    if (!orderedQueryable)
                    {
                        if (!string.IsNullOrWhiteSpace(sorting.FieldName))
                        {
                            source = source.OrderByDescending(sorting.FieldName, out var success);

                            if (success)
                            {
                                orderedQueryable = true;
                            }
                        }
                        else if (sorting.KeySelector is not null)
                        {
                            source = sorting.KeySelector.Invoke(source);

                            orderedQueryable = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(sorting.FieldName))
                        {
                            source = ((IOrderedQueryable<T>)source).ThenByDescending(sorting.FieldName, out _);
                        }
                        else if (sorting.KeySelector is not null)
                        {
                            source = sorting.KeySelector.Invoke(source);
                        }
                    }
                }
            }

            return source;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string fieldName, out bool success)
            where T : class
        {
            var expression = GenerateMethodCall(source, nameof(OrderBy), fieldName, out success);

            var queryable = (expression is null ? source : source.Provider.CreateQuery<T>(expression)) as IOrderedQueryable<T>;

            return queryable;
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string fieldName, out bool success)
            where T : class
        {
            var expression = GenerateMethodCall(source, nameof(OrderByDescending), fieldName, out success);

            var queryable = (expression is null ? source : source.Provider.CreateQuery<T>(expression)) as IOrderedQueryable<T>;

            return queryable;
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string fieldName, out bool success)
            where T : class
        {
            var expression = GenerateMethodCall(source, nameof(ThenBy), fieldName, out success);

            var queryable = expression is null ? source : source.Provider.CreateQuery<T>(expression) as IOrderedQueryable<T>;

            return queryable;
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string fieldName, out bool success)
            where T : class
        {
            var expression = GenerateMethodCall(source, nameof(ThenByDescending), fieldName, out success);

            var queryable = expression is null ? source : source.Provider.CreateQuery<T>(expression) as IOrderedQueryable<T>;

            return queryable;
        }

        private static MethodCallExpression GenerateMethodCall<T>(IQueryable<T> source, string methodName, string fieldName, out bool success)
            where T : class
        {
            try
            {
                var parameter = Expression.Parameter(typeof(T), "keySelector");

                var body = fieldName.Split('.').Aggregate<string, Expression>(parameter, Expression.PropertyOrField);

                var selector = Expression.Lambda(body, parameter);

                if (success = selector is not null)
                {
                    var expression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(T), body.Type }, source.Expression, selector);

                    return expression;
                }
            }
            catch { success = false; }

            return null;
        }
    }
}
