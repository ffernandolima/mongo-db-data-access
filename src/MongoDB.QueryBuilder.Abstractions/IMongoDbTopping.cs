
namespace MongoDB.QueryBuilder
{
    public interface IMongoDbTopping
    {
        int? TopRows { get; }
        bool IsEnabled { get; }
    }
}
