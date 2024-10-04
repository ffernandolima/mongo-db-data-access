using MongoDB.Driver;
using System;
using System.Collections.Concurrent;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbDatabaseManager : IMongoDbDatabaseManager
    {
        private readonly ConcurrentDictionary<string, IMongoDatabase> _databases;

        private static readonly Lazy<MongoDbDatabaseManager> _factory = new(() =>
            new MongoDbDatabaseManager(), isThreadSafe: true);

        public static MongoDbDatabaseManager Instance => _factory.Value;

        public MongoDbDatabaseManager()
        {
            _databases = new ConcurrentDictionary<string, IMongoDatabase>();
        }

        public IMongoDatabase GetOrCreate(IMongoClient client, string databaseName, MongoDatabaseSettings databaseSettings = null)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client), $"{nameof(client)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            var lookupKey = $"{client.Settings.ToString()}-{databaseName.ToLower()}";
            return _databases.GetOrAdd(lookupKey, _ => client.GetDatabase(databaseName, databaseSettings));
        }
    }
}
