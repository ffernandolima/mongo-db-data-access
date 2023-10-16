using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbThrottlingSemaphoreManager : IMongoDbThrottlingSemaphoreManager
    {
        private readonly IMongoDbThrottlingSemaphoreFactory _semaphoreFactory;
        private readonly ConcurrentDictionary<IMongoClient, IMongoDbThrottlingSemaphore> _semaphores;

        private static readonly Lazy<MongoDbThrottlingSemaphoreManager> _factory = new(() =>
            new MongoDbThrottlingSemaphoreManager(), isThreadSafe: true);

        public static MongoDbThrottlingSemaphoreManager Instance => _factory.Value;

        public MongoDbThrottlingSemaphoreManager()
            : this(MongoDbThrottlingSemaphoreFactory.Instance)
        { }

        public MongoDbThrottlingSemaphoreManager(IMongoDbThrottlingSemaphoreFactory semaphoreFactory)
        {
            _semaphoreFactory = semaphoreFactory ?? throw new ArgumentNullException(nameof(semaphoreFactory), $"{nameof(semaphoreFactory)} cannot be null.");
            _semaphores = new ConcurrentDictionary<IMongoClient, IMongoDbThrottlingSemaphore>();
        }

        public IMongoDbThrottlingSemaphore GetOrCreate(IMongoClient client, int maximumNumberOfConcurrentRequests)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client), $"{nameof(client)} cannot be null.");
            }

            if (TryGet(client, out IMongoDbThrottlingSemaphore semaphore))
            {
                return semaphore;
            }

            _semaphores[client] = semaphore = _semaphoreFactory.Create(maximumNumberOfConcurrentRequests);

            return semaphore;
        }

        private bool TryGet(IMongoClient client, out IMongoDbThrottlingSemaphore semaphore)
        {
            semaphore = _semaphores.ToArray()
                                   .Where(semaphore => semaphore.Key.Settings.Servers.Count() == client.Settings.Servers.Count())
                                   .Where(semaphore => semaphore.Key.Settings.Servers.All(client.Settings.Servers.Contains))
                                   .Select(semaphore => semaphore.Value)
                                   .SingleOrDefault();

            return semaphore is not null;
        }
    }
}
