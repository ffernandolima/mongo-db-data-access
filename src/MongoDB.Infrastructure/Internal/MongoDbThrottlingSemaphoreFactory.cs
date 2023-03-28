using System;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbThrottlingSemaphoreFactory
    {
        private static readonly object _sync = new();

        private static readonly Lazy<MongoDbThrottlingSemaphoreFactory> _factory = new(() =>
            new MongoDbThrottlingSemaphoreFactory(), isThreadSafe: true);

        private IMongoDbThrottlingSemaphore _semaphore;

        public static MongoDbThrottlingSemaphoreFactory Instance => _factory.Value;

        public IMongoDbThrottlingSemaphore GetOrCreate(int maximumNumberOfConcurrentRequests)
        {
            lock (_sync)
            {
                return _semaphore ??= maximumNumberOfConcurrentRequests > 0
                    ? new MongoDbThrottlingSemaphore(maximumNumberOfConcurrentRequests)
                    : MongoDbNoopThrottlingSemaphore.Instance;
            }
        }
    }
}
