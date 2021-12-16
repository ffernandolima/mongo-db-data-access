
namespace MongoDB.QueryBuilder
{
    public interface IMongoDbPaging
    {
        int? PageIndex { get; }
        int? PageSize { get; }
        int TotalCount { get; }
        bool IsEnabled { get; }
    }
}
