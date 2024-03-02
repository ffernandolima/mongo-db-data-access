using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbClientManager : IMongoDbClientManager
    {
        private readonly ConcurrentBag<IMongoClient> _clients;

        private static readonly Lazy<MongoDbClientManager> _factory = new(() =>
            new MongoDbClientManager(), isThreadSafe: true);

        public static MongoDbClientManager Instance => _factory.Value;

        public MongoDbClientManager()
        {
            _clients = new ConcurrentBag<IMongoClient>();
        }

        public IMongoClient GetOrCreate(MongoClientSettings clientSettings)
        {
            if (clientSettings is null)
            {
                throw new ArgumentNullException(nameof(clientSettings), $"{nameof(clientSettings)} cannot be null.");
            }

            if (TryGet(clientSettings, out IMongoClient client))
            {
                return client;
            }

            _clients.Add(client = new MongoClient(clientSettings));

            return client;
        }

        private bool TryGet(MongoClientSettings clientSettings, out IMongoClient client)
        {
            client = _clients.Where(client => client is not null)
                             .Where(client => client.Settings.ToString() == clientSettings.ToString())
                             .SingleOrDefault();

            return client is not null;
        }
    }
}
