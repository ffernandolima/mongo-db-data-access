using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbThrottlingSemaphoreManager : IMongoDbThrottlingSemaphoreManager
    {
        private readonly IMongoDbThrottlingSemaphoreFactory _semaphoreFactory;
        private readonly ConcurrentDictionary<string, IMongoDbThrottlingSemaphore> _semaphores;
        private readonly object _lock = new object();

        private static readonly Lazy<MongoDbThrottlingSemaphoreManager> _factory = new(() =>
            new MongoDbThrottlingSemaphoreManager(), isThreadSafe: true);

        public static MongoDbThrottlingSemaphoreManager Instance => _factory.Value;

        public MongoDbThrottlingSemaphoreManager()
            : this(MongoDbThrottlingSemaphoreFactory.Instance)
        { }

        public MongoDbThrottlingSemaphoreManager(IMongoDbThrottlingSemaphoreFactory semaphoreFactory)
        {
            _semaphoreFactory = semaphoreFactory ?? throw new ArgumentNullException(nameof(semaphoreFactory), $"{nameof(semaphoreFactory)} cannot be null.");
            _semaphores = new ConcurrentDictionary<string, IMongoDbThrottlingSemaphore>();
        }

        public IMongoDbThrottlingSemaphore GetOrCreate(IMongoClient client, int maximumNumberOfConcurrentRequests)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client), $"{nameof(client)} cannot be null.");
            }

            // Use lock because the search TryGet is complex search and there is no way to do atomic search and Insert
            lock (_lock)
            {
                if (TryGet(client, out IMongoDbThrottlingSemaphore semaphore))
                {
                    return semaphore;
                }

                var cluster = new MongoDbCluster(client.Settings.Servers);
                _semaphores[cluster] = semaphore = _semaphoreFactory.Create(maximumNumberOfConcurrentRequests);
                
                return semaphore;
            };            
        }

        private bool TryGet(IMongoClient client, out IMongoDbThrottlingSemaphore semaphore)
        {
            semaphore = _semaphores.ToArray()
                                   .Where(semaphore => semaphore.Key is not null && semaphore.Value is not null)
                                   .Where(semaphore => semaphore.Key == new MongoDbCluster(client.Settings.Servers))
                                   .Select(semaphore => semaphore.Value)
                                   .SingleOrDefault();

            return semaphore is not null;
        }
    }
}
