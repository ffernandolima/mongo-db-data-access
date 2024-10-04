using MongoDB.Driver;
using System;
using System.Collections.Concurrent;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbClientManager : IMongoDbClientManager
    {
        private readonly ConcurrentDictionary<string, IMongoClient> _clients;

        private static readonly Lazy<MongoDbClientManager> _factory = new(() =>
            new MongoDbClientManager(), isThreadSafe: true);

        public static MongoDbClientManager Instance => _factory.Value;

        public MongoDbClientManager()
        {
            _clients = new ConcurrentDictionary<string, IMongoClient>(StringComparer.OrdinalIgnoreCase);
        }

        public IMongoClient GetOrCreate(MongoClientSettings clientSettings)
        {
            if (clientSettings is null)
            {
                throw new ArgumentNullException(nameof(clientSettings), $"{nameof(clientSettings)} cannot be null.");
            }

            return _clients.GetOrAdd(clientSettings.ToString(), _ => new MongoClient(clientSettings));
        }
    }
}
