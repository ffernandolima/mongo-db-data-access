
namespace MongoDB.QueryBuilder
{
    public interface IMongoDbSingleResultQuery<T> : IMongoDbQuery<T> where T : class
    { }

    public interface IMongoDbSingleResultQuery<T, TResult> : IMongoDbQuery<T, TResult> where T : class
    { }
}
