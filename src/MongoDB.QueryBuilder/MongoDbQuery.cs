using LinqKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public abstract class MongoDbQuery<T, TBuilder> : IMongoDbQuery<T>, IMongoDbQueryBuilder<T, TBuilder>
        where T : class
        where TBuilder : IMongoDbQueryBuilder<T, TBuilder>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract TBuilder BuilderInstance { get; }

        #region Ctor

        internal MongoDbQuery()
        { }

        #endregion Ctor

        #region IMongoDbQuery<T> Members

        public Expression<Func<T, bool>> Predicate { get; internal set; } = PredicateBuilder.New<T>(defaultExpression: true);
        public IList<IMongoDbSorting<T>> Sortings { get; internal set; } = new List<IMongoDbSorting<T>>();
        public Expression<Func<T, T>> Selector { get; internal set; }

        #endregion IMongoDbQuery<T> Members

        #region IMongoDbQueryBuilder<T, TBuilder> Members

        public TBuilder AndFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.And(predicate);
            }

            return BuilderInstance;
        }

        public TBuilder OrFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.Or(predicate);
            }

            return BuilderInstance;
        }

        public TBuilder OrderBy(Expression<Func<T, object>> keySelector)
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

            return BuilderInstance;
        }

        public TBuilder ThenBy(Expression<Func<T, object>> keySelector)
            => OrderBy(keySelector);

        public TBuilder OrderBy(string fieldName)
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

            return BuilderInstance;
        }

        public TBuilder ThenBy(string fieldName)
            => OrderBy(fieldName);

        public TBuilder OrderByDescending(Expression<Func<T, object>> keySelector)
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

            return BuilderInstance;
        }

        public TBuilder ThenByDescending(Expression<Func<T, object>> keySelector)
            => OrderByDescending(keySelector);

        public TBuilder OrderByDescending(string fieldName)
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

            return BuilderInstance;
        }

        public TBuilder ThenByDescending(string fieldName)
            => OrderByDescending(fieldName);

        public TBuilder Select(Expression<Func<T, T>> selector)
        {
            if (selector is not null)
            {
                Selector = selector;
            }

            return BuilderInstance;
        }

        #endregion IMongoDbQueryBuilder<T, TBuilder> Members
    }

    public abstract class MongoDbQuery<T, TResult, TBuilder> : IMongoDbQuery<T, TResult>, IMongoDbQueryBuilder<T, TResult, TBuilder>
        where T : class
        where TBuilder : IMongoDbQueryBuilder<T, TResult, TBuilder>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract TBuilder BuilderInstance { get; }

        #region Ctor

        internal MongoDbQuery()
        { }

        #endregion Ctor

        #region IMongoDbQuery<T, TResult> Members

        public Expression<Func<T, bool>> Predicate { get; internal set; } = PredicateBuilder.New<T>(defaultExpression: true);
        public IList<IMongoDbSorting<T>> Sortings { get; internal set; } = new List<IMongoDbSorting<T>>();
        public Expression<Func<T, TResult>> Selector { get; internal set; }

        #endregion IMongoDbQuery<T, TResult> Members

        #region IMongoDbQueryBuilder<T, TResult, TBuilder> Members

        public TBuilder AndFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.And(predicate);
            }

            return BuilderInstance;
        }

        public TBuilder OrFilter(Expression<Func<T, bool>> predicate)
        {
            if (predicate is not null)
            {
                Predicate = Predicate.Or(predicate);
            }

            return BuilderInstance;
        }

        public TBuilder OrderBy(Expression<Func<T, object>> keySelector)
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

            return BuilderInstance;
        }

        public TBuilder ThenBy(Expression<Func<T, object>> keySelector)
            => OrderBy(keySelector);

        public TBuilder OrderBy(string fieldName)
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

            return BuilderInstance;
        }

        public TBuilder ThenBy(string fieldName)
            => OrderBy(fieldName);

        public TBuilder OrderByDescending(Expression<Func<T, object>> keySelector)
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

            return BuilderInstance;
        }

        public TBuilder ThenByDescending(Expression<Func<T, object>> keySelector)
            => OrderByDescending(keySelector);

        public TBuilder OrderByDescending(string fieldName)
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

            return BuilderInstance;
        }

        public TBuilder ThenByDescending(string fieldName)
            => OrderByDescending(fieldName);

        public TBuilder Select(Expression<Func<T, TResult>> selector)
        {
            if (selector is not null)
            {
                Selector = selector;
            }

            return BuilderInstance;
        }

        #endregion IMongoDbQueryBuilder<T, TResult, TBuilder> Members
    }
}
