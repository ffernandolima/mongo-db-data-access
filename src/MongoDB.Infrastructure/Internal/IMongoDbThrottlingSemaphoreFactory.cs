namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbThrottlingSemaphoreFactory
    {
        IMongoDbThrottlingSemaphore Create(int maximumNumberOfConcurrentRequests);
        IMongoDbThrottlingSemaphore GetOrCreate(int maximumNumberOfConcurrentRequests);
    }
}
