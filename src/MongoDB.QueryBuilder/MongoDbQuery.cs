using LinqKit;
using MongoDB.QueryBuilder.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public abstract class MongoDbQuery<T> : IMongoDbQuery<T> where T : class
    {
        #region Ctor

        internal MongoDbQuery()
        { }

        #endregion Ctor

        #region IMongoDbQuery<T> Members

        public Expression<Func<T, bool>> Predicate { get; internal set; } = PredicateBuilder.New<T>(defaultExpression: true);
        public IList<IMongoDbSorting<T>> Sortings { get; internal set; } = new List<IMongoDbSorting<T>>();
        public Expression<Func<T, T>> Selector { get; internal set; }

        public IMongoDbQuery<T> AndFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.And(predicate);
            }

            return this;
        }

        public IMongoDbQuery<T> OrFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.Or(predicate);
            }

            return this;
        }

        public IMongoDbQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.OrderBy,
                    SortingDirection = MongoDbSortingDirection.Ascending,
                    KeySelector = queryable => queryable.OrderBy(keySelector)
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.ThenBy,
                    SortingDirection = MongoDbSortingDirection.Ascending,
                    KeySelector = queryable => ((IOrderedQueryable<T>)queryable).ThenBy(keySelector)
                };

                Sortings.Validate(MongoDbSortingType.ThenBy);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> OrderBy(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.OrderBy,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenBy(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.ThenBy,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Validate(MongoDbSortingType.ThenBy);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.OrderByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending,
                    KeySelector = queryable => queryable.OrderByDescending(keySelector)
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.ThenByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending,
                    KeySelector = queryable => ((IOrderedQueryable<T>)queryable).ThenByDescending(keySelector)
                };

                Sortings.Validate(MongoDbSortingType.ThenByDescending);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> OrderByDescending(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.OrderByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenByDescending(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.ThenByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Validate(MongoDbSortingType.ThenByDescending);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> Select(Expression<Func<T, T>> selector)
        {
            if (selector is not null)
            {
                Selector = selector;
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
            => this.ToQuery(selector);

        #endregion IMongoDbQuery<T> Members
    }

    public abstract class MongoDbQuery<T, TResult> : IMongoDbQuery<T, TResult> where T : class
    {
        #region Ctor

        internal MongoDbQuery()
        { }

        #endregion Ctor

        #region IMongoDbQuery<T, TResult> Members

        public Expression<Func<T, bool>> Predicate { get; internal set; } = PredicateBuilder.New<T>(defaultExpression: true);
        public IList<IMongoDbSorting<T>> Sortings { get; internal set; } = new List<IMongoDbSorting<T>>();
        public Expression<Func<T, TResult>> Selector { get; internal set; }

        public IMongoDbQuery<T, TResult> AndFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.And(predicate);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> OrFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.Or(predicate);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.OrderBy,
                    SortingDirection = MongoDbSortingDirection.Ascending,
                    KeySelector = queryable => queryable.OrderBy(keySelector)
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.ThenBy,
                    SortingDirection = MongoDbSortingDirection.Ascending,
                    KeySelector = queryable => ((IOrderedQueryable<T>)queryable).ThenBy(keySelector)
                };

                Sortings.Validate(MongoDbSortingType.ThenBy);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> OrderBy(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.OrderBy,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenBy(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.ThenBy,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Validate(MongoDbSortingType.ThenBy);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.OrderByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending,
                    KeySelector = queryable => queryable.OrderByDescending(keySelector)
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    SortingType = MongoDbSortingType.ThenByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending,
                    KeySelector = queryable => ((IOrderedQueryable<T>)queryable).ThenByDescending(keySelector)
                };

                Sortings.Validate(MongoDbSortingType.ThenByDescending);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> OrderByDescending(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.OrderByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenByDescending(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingType = MongoDbSortingType.ThenByDescending,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Validate(MongoDbSortingType.ThenByDescending);
                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> Select(Expression<Func<T, TResult>> selector)
        {
            if (selector is not null)
            {
                Selector = selector;
            }

            return this;
        }

        #endregion IMongoDbQuery<T, TResult> Members
    }
}
