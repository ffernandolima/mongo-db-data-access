using LinqKit;
using MongoDB.QueryBuilder.Internal;
using System;
using System.Collections.Generic;
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

        public IMongoDbQuery<T> OrderBy(Expression<Func<T, object>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    KeySelector = keySelector,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenBy(Expression<Func<T, object>> keySelector)
            => OrderBy(keySelector);

        public IMongoDbQuery<T> OrderBy(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenBy(string fieldName)
            => OrderBy(fieldName);

        public IMongoDbQuery<T> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    KeySelector = keySelector,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenByDescending(Expression<Func<T, object>> keySelector)
            => OrderByDescending(keySelector);

        public IMongoDbQuery<T> OrderByDescending(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T> ThenByDescending(string fieldName)
            => OrderByDescending(fieldName);

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

        public IMongoDbQuery<T, TResult> OrderBy(Expression<Func<T, object>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    KeySelector = keySelector,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenBy(Expression<Func<T, object>> keySelector)
            => OrderBy(keySelector);

        public IMongoDbQuery<T, TResult> OrderBy(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingDirection = MongoDbSortingDirection.Ascending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenBy(string fieldName)
            => OrderBy(fieldName);

        public IMongoDbQuery<T, TResult> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            if (keySelector is not null)
            {
                var sorting = new MongoDbSorting<T>
                {
                    KeySelector = keySelector,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenByDescending(Expression<Func<T, object>> keySelector)
            => OrderByDescending(keySelector);

        public IMongoDbQuery<T, TResult> OrderByDescending(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                var sorting = new MongoDbSorting<T>
                {
                    FieldName = fieldName,
                    SortingDirection = MongoDbSortingDirection.Descending
                };

                Sortings.Add(sorting);
            }

            return this;
        }

        public IMongoDbQuery<T, TResult> ThenByDescending(string fieldName)
            => OrderByDescending(fieldName);

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
