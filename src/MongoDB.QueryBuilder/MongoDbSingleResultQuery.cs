namespace MongoDB.QueryBuilder
{
    public class MongoDbSingleResultQuery<T> : MongoDbQuery<T>, IMongoDbSingleResultQuery<T> where T : class
    {
        public static IMongoDbSingleResultQuery<T> New() => new MongoDbSingleResultQuery<T>();

        #region Ctor

        internal MongoDbSingleResultQuery()
        { }

        #endregion Ctor
    }

    public class MongoDbSingleResultQuery<T, TResult> : MongoDbQuery<T, TResult>, IMongoDbSingleResultQuery<T, TResult> where T : class
    {
        public static IMongoDbSingleResultQuery<T, TResult> New() => new MongoDbSingleResultQuery<T, TResult>();

        #region Ctor

        internal MongoDbSingleResultQuery()
        { }

        #endregion Ctor
    }
}
