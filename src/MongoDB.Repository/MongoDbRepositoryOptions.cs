namespace MongoDB.Repository
{
    public class MongoDbRepositoryOptions<T> : IMongoDbRepositoryOptions<T> where T : class
    {
        public static readonly MongoDbRepositoryOptions<T> Default = new();

        public string CollectionName { get; set; } = typeof(T).Name;
    }
}
