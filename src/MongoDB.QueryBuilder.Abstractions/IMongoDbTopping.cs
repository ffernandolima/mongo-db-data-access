
namespace MongoDB.QueryBuilder.Abstractions
{
    public interface IMongoDbTopping
    {
        int? TopRows { get; }
        bool IsEnabled { get; }
    }
}
