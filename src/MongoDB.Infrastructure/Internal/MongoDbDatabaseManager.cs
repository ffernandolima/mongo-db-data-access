using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbDatabaseManager : IMongoDbDatabaseManager
    {
        private readonly ConcurrentBag<IMongoDatabase> _databases;

        private static readonly Lazy<MongoDbDatabaseManager> _factory = new(() =>
            new MongoDbDatabaseManager(), isThreadSafe: true);

        public static MongoDbDatabaseManager Instance => _factory.Value;

        public MongoDbDatabaseManager()
        {
            _databases = new ConcurrentBag<IMongoDatabase>();
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

            if (TryGet(client, databaseName, out IMongoDatabase database))
            {
                return database;
            }

            _databases.Add(database = client.GetDatabase(databaseName, databaseSettings));

            return database;
        }

        private bool TryGet(IMongoClient client, string databaseName, out IMongoDatabase database)
        {
            database = _databases.Where(database => database is not null)
                                 .Where(database => database.Client.Settings.ToString() == client.Settings.ToString())
                                 .Where(database => database.Client.Settings.Servers.Count() == client.Settings.Servers.Count())
                                 .Where(database => database.Client.Settings.Servers.All(client.Settings.Servers.Contains))
                                 .Where(database => database.DatabaseNamespace.DatabaseName.Equals(databaseName, StringComparison.OrdinalIgnoreCase))
                                 .SingleOrDefault();

            return database is not null;
        }
    }
}
