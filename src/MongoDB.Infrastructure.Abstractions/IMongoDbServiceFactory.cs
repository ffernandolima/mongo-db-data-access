namespace MongoDB.Infrastructure
{
    public interface IMongoDbServiceFactory
    {
        T GetService<T>() where T : class;
    }
}
