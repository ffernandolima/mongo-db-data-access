using MongoDB.Driver;

namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbThrottlingSemaphoreManager
    {
        IMongoDbThrottlingSemaphore GetOrCreate(IMongoClient client, int maximumNumberOfConcurrentRequests);
    }
}
