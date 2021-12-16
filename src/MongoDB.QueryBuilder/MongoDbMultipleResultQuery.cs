namespace MongoDB.QueryBuilder
{
    public class MongoDbMultipleResultQuery<T> : MongoDbQuery<T>, IMongoDbMultipleResultQuery<T> where T : class
    {
        public static IMongoDbMultipleResultQuery<T> New() => new MongoDbMultipleResultQuery<T>();

        #region Ctor

        internal MongoDbMultipleResultQuery()
        { }

        #endregion Ctor

        #region IMongoDbMultipleResultQuery<T> Members

        public IMongoDbPaging Paging { get; internal set; } = new MongoDbPaging();
        public IMongoDbTopping Topping { get; internal set; } = new MongoDbTopping();

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

        #endregion IMongoDbMultipleResultQuery<T> Members
    }

    public class MongoDbMultipleResultQuery<T, TResult> : MongoDbQuery<T, TResult>, IMongoDbMultipleResultQuery<T, TResult> where T : class
    {
        public static IMongoDbMultipleResultQuery<T, TResult> New() => new MongoDbMultipleResultQuery<T, TResult>();

        #region Ctor

        internal MongoDbMultipleResultQuery()
        { }

        #endregion Ctor

        #region IMongoDbMultipleResultQuery<T, TResult> Members

        public IMongoDbPaging Paging { get; internal set; } = new MongoDbPaging();
        public IMongoDbTopping Topping { get; internal set; } = new MongoDbTopping();

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
