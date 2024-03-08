using MongoDB.QueryBuilder.Internal;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public class MongoDbMultipleResultQuery<T> : MongoDbQuery<T, IMongoDbMultipleResultQuery<T>>, IMongoDbMultipleResultQuery<T> where T : class
    {
        public static IMongoDbMultipleResultQuery<T> New() => new MongoDbMultipleResultQuery<T>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override IMongoDbMultipleResultQuery<T> BuilderInstance => this;

        #region Ctor

        internal MongoDbMultipleResultQuery()
        { }

        #endregion Ctor

        #region IMongoDbMultipleResultQuery Members

        public IMongoDbPaging Paging { get; internal set; } = new MongoDbPaging();
        public IMongoDbTopping Topping { get; internal set; } = new MongoDbTopping();

        #endregion IMongoDbMultipleResultQuery Members

        #region IMongoDbMultipleResultQuery<T> Members

        public IMongoDbMultipleResultQuery<T> Page(int? pageIndex, int? pageSize)
        {
            if (Paging is MongoDbPaging paging)
            {
                paging.PageIndex = pageIndex;
                paging.PageSize = pageSize;
            }

            return this;
        }

        public IMongoDbMultipleResultQuery<T> Top(int? topRows)
        {
            if (Topping is MongoDbTopping topping)
            {
                topping.TopRows = topRows;
            }

            return this;
        }

        public IMongoDbMultipleResultQuery<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
            => this.ToQuery(selector);

        #endregion IMongoDbMultipleResultQuery<T> Members
    }

    public class MongoDbMultipleResultQuery<T, TResult> : MongoDbQuery<T, TResult, IMongoDbMultipleResultQuery<T, TResult>>, IMongoDbMultipleResultQuery<T, TResult> where T : class
    {
        public static IMongoDbMultipleResultQuery<T, TResult> New() => new MongoDbMultipleResultQuery<T, TResult>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override IMongoDbMultipleResultQuery<T, TResult> BuilderInstance => this;

        #region Ctor

        internal MongoDbMultipleResultQuery()
        { }

        #endregion Ctor

        #region IMongoDbMultipleResultQuery Members

        public IMongoDbPaging Paging { get; internal set; } = new MongoDbPaging();
        public IMongoDbTopping Topping { get; internal set; } = new MongoDbTopping();

        #endregion IMongoDbMultipleResultQuery Members

        #region IMongoDbMultipleResultQuery<T, TResult> Members

        public IMongoDbMultipleResultQuery<T, TResult> Page(int? pageIndex, int? pageSize)
        {
            if (Paging is MongoDbPaging paging)
            {
                paging.PageIndex = pageIndex;
                paging.PageSize = pageSize;
            }

            return this;
        }

        public IMongoDbMultipleResultQuery<T, TResult> Top(int? topRows)
        {
            if (Topping is MongoDbTopping topping)
            {
                topping.TopRows = topRows;
            }

            return this;
        }

        #endregion IMongoDbMultipleResultQuery<T, TResult> Members
    }
}
