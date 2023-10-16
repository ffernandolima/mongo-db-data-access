using System;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbThrottlingSemaphoreFactory : IMongoDbThrottlingSemaphoreFactory
    {
        private static readonly object _sync = new();

        private static readonly Lazy<MongoDbThrottlingSemaphoreFactory> _factory = new(() =>
            new MongoDbThrottlingSemaphoreFactory(), isThreadSafe: true);

        public static MongoDbThrottlingSemaphoreFactory Instance => _factory.Value;

        public IMongoDbThrottlingSemaphore Create(int maximumNumberOfConcurrentRequests)
        {
            return maximumNumberOfConcurrentRequests > 0
                ? new MongoDbThrottlingSemaphore(maximumNumberOfConcurrentRequests)
                : MongoDbNoopThrottlingSemaphore.Instance;
        }
    }
}
