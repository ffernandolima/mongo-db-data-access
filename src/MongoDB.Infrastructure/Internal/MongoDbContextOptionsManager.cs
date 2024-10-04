using System;
using System.Collections.Concurrent;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbContextOptionsManager : IMongoDbContextOptionsManager
    {
        private readonly ConcurrentDictionary<string, IMongoDbContextOptions> _options;

        private static readonly Lazy<MongoDbContextOptionsManager> _factory = new(() =>
            new MongoDbContextOptionsManager(), isThreadSafe: true);

        public static MongoDbContextOptionsManager Instance => _factory.Value;

        public MongoDbContextOptionsManager()
        {
            _options = new ConcurrentDictionary<string, IMongoDbContextOptions>();
        }

        public IMongoDbContextOptions GetOrAdd(IMongoDbContextOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(options.DbContextId))
            {
                throw new ArgumentException($"{nameof(options.DbContextId)} cannot be null or whitespace.", nameof(options.DbContextId));
            }

            return _options.GetOrAdd(options.DbContextId.ToLower(), _ => options);
        }
    }
}
