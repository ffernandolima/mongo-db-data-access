using MongoDB.QueryBuilder.Internal;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public class MongoDbSingleResultQuery<T> : MongoDbQuery<T, IMongoDbSingleResultQuery<T>>, IMongoDbSingleResultQuery<T> where T : class
    {
        public static IMongoDbSingleResultQuery<T> New() => new MongoDbSingleResultQuery<T>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override IMongoDbSingleResultQuery<T> BuilderInstance => this;

        #region Ctor

        internal MongoDbSingleResultQuery()
        { }

        #endregion Ctor

        #region IMongoDbSingleResultQuery<T> Members

        public IMongoDbSingleResultQuery<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
            => this.ToQuery(selector);

        #endregion IMongoDbSingleResultQuery<T> Members
    }

    public class MongoDbSingleResultQuery<T, TResult> : MongoDbQuery<T, TResult, IMongoDbSingleResultQuery<T, TResult>>, IMongoDbSingleResultQuery<T, TResult> where T : class
    {
        public static IMongoDbSingleResultQuery<T, TResult> New() => new MongoDbSingleResultQuery<T, TResult>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected override IMongoDbSingleResultQuery<T, TResult> BuilderInstance => this;

        #region Ctor

        internal MongoDbSingleResultQuery()
        { }

        #endregion Ctor
    }
}
