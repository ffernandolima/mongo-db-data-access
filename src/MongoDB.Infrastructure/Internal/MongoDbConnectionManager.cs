using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbConnectionManager : IMongoDbConnectionManager
    {
        private readonly IMongoDbClientManager _clientManager;
        private readonly IMongoDbDatabaseManager _databaseManager;
        private readonly IMongoDbContextOptionsManager _optionsManager;
        private readonly IMongoDbThrottlingSemaphoreManager _semaphoreManager;

        private readonly IDictionary<string, MongoDbConnection> _connections;

        private static readonly Lazy<MongoDbConnectionManager> _factory = new(() =>
            new MongoDbConnectionManager(
                MongoDbClientManager.Instance,
                MongoDbDatabaseManager.Instance,
                MongoDbContextOptionsManager.Instance,
                MongoDbThrottlingSemaphoreManager.Instance),
            isThreadSafe: true);

        public static MongoDbConnectionManager Instance => _factory.Value;

        public MongoDbConnectionManager(
            IMongoDbClientManager clientManager,
            IMongoDbDatabaseManager databaseManager,
            IMongoDbContextOptionsManager optionsManager,
            IMongoDbThrottlingSemaphoreManager semaphoreManager)
        {
            _clientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager), $"{nameof(clientManager)} cannot be null.");
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager), $"{nameof(databaseManager)} cannot be null.");
            _optionsManager = optionsManager ?? throw new ArgumentNullException(nameof(optionsManager), $"{nameof(optionsManager)} cannot be null.");
            _semaphoreManager = semaphoreManager ?? throw new ArgumentNullException(nameof(semaphoreManager), $"{nameof(semaphoreManager)} cannot be null.");
            _connections = new Dictionary<string, MongoDbConnection>();
        }

        public MongoDbConnection GetOrCreate(
            MongoClientSettings clientSettings,
            string databaseName,
            MongoDatabaseSettings databaseSettings,
            MongoDbContextOptions dbContextOptions)
        {
            if (dbContextOptions is null)
            {
                throw new ArgumentNullException(nameof(dbContextOptions), $"{nameof(dbContextOptions)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(dbContextOptions.DbContextId))
            {
                throw new ArgumentException($"{nameof(dbContextOptions.DbContextId)} cannot be null or whitespace.", nameof(dbContextOptions.DbContextId));
            }

            if (_connections.TryGetValue(dbContextOptions.DbContextId, out var connection))
            {
                return connection;
            }

            var client = _clientManager.GetOrCreate(clientSettings);
            var database = _databaseManager.GetOrCreate(client, databaseName, databaseSettings);
            var options = _optionsManager.GetOrAdd(dbContextOptions);
            var semaphore = _semaphoreManager.GetOrCreate(client, options.MaximumNumberOfConcurrentRequests);

            connection = new MongoDbConnection
            {
                Client = client,
                Database = database,
                Options = options,
                Semaphore = semaphore
            };

            _connections[options.DbContextId] = connection;

            return connection;
        }
    }
}

