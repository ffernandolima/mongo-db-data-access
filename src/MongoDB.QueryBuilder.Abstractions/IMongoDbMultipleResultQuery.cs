
namespace MongoDB.QueryBuilder.Abstractions
{
    public interface IMongoDbMultipleResultQuery
    {
        IMongoDbPaging Paging { get; }
        IMongoDbTopping Topping { get; }
    }

    public interface IMongoDbMultipleResultQuery<T> : IMongoDbMultipleResultQuery, IMongoDbQuery<T> where T : class
    {
        IMongoDbMultipleResultQuery<T> Page(int? pageIndex, int? pageSize);
        IMongoDbMultipleResultQuery<T> Top(int? topRows);
    }

    public interface IMongoDbMultipleResultQuery<T, TResult> : IMongoDbMultipleResultQuery, IMongoDbQuery<T, TResult> where T : class
    {
        IMongoDbMultipleResultQuery<T, TResult> Page(int? pageIndex, int? pageSize);
        IMongoDbMultipleResultQuery<T, TResult> Top(int? topRows);
    }
}
