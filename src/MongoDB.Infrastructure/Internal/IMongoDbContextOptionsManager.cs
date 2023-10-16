namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbContextOptionsManager
    {
        IMongoDbContextOptions GetOrAdd(IMongoDbContextOptions options);
    }
}
