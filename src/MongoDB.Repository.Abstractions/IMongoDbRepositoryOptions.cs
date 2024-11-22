namespace MongoDB.Repository
{
    public interface IMongoDbRepositoryOptions<T> where T : class
    {
        string CollectionName { get; set; }
    }
}
