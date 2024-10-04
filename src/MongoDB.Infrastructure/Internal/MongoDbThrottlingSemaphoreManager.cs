using MongoDB.Driver;
using System;
using System.Collections.Concurrent;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbThrottlingSemaphoreManager : IMongoDbThrottlingSemaphoreManager
    {
        private readonly IMongoDbThrottlingSemaphoreFactory _semaphoreFactory;
        private readonly ConcurrentDictionary<string, IMongoDbThrottlingSemaphore> _semaphores;

        private static readonly Lazy<MongoDbThrottlingSemaphoreManager> _factory = new(() =>
            new MongoDbThrottlingSemaphoreManager(), isThreadSafe: true);

        public static MongoDbThrottlingSemaphoreManager Instance => _factory.Value;

        public MongoDbThrottlingSemaphoreManager()
            : this(MongoDbThrottlingSemaphoreFactory.Instance)
        { }

        public MongoDbThrottlingSemaphoreManager(IMongoDbThrottlingSemaphoreFactory semaphoreFactory)
        {
            _semaphoreFactory = semaphoreFactory ?? throw new ArgumentNullException(nameof(semaphoreFactory), $"{nameof(semaphoreFactory)} cannot be null.");
            _semaphores = new ConcurrentDictionary<string, IMongoDbThrottlingSemaphore>(StringComparer.OrdinalIgnoreCase);
        }

        public IMongoDbThrottlingSemaphore GetOrCreate(IMongoClient client, int maximumNumberOfConcurrentRequests)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client), $"{nameof(client)} cannot be null.");
            }

            return _semaphores.GetOrAdd(
                new MongoDbCluster(client.Settings.Servers),
                _ => _semaphoreFactory.Create(maximumNumberOfConcurrentRequests));
        }
    }
}
