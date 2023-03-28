namespace MongoDB.Infrastructure
{
    public interface IMongoDbContextOptions
    {
        bool AcceptAllChangesOnSave { get; }
        int MaximumNumberOfConcurrentRequests { get; }
    }
}
