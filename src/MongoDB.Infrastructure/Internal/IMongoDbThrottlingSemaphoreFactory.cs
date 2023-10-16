namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbThrottlingSemaphoreFactory
    {
        IMongoDbThrottlingSemaphore Create(int maximumNumberOfConcurrentRequests);
    }
}
