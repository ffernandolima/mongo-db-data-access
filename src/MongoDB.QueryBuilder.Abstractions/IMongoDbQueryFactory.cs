
namespace MongoDB.QueryBuilder
{
    public interface IMongoDbQueryFactory<T> where T : class
    {
        IMongoDbSingleResultQuery<T> SingleResultQuery();
        IMongoDbMultipleResultQuery<T> MultipleResultQuery();

        IMongoDbSingleResultQuery<T, TResult> SingleResultQuery<TResult>();
        IMongoDbMultipleResultQuery<T, TResult> MultipleResultQuery<TResult>();
    }
}
