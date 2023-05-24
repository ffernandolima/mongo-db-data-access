using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbContextOptionsManager : IMongoDbContextOptionsManager
    {
        private readonly ICollection<IMongoDbContextOptions> _options;

        private static readonly Lazy<MongoDbContextOptionsManager> _factory = new(() =>
            new MongoDbContextOptionsManager(), isThreadSafe: true);

        public static MongoDbContextOptionsManager Instance => _factory.Value;

        public MongoDbContextOptionsManager()
        {
            _options = new Collection<IMongoDbContextOptions>();
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

            if (TryGet(options.DbContextId, out IMongoDbContextOptions cachedOptions))
            {
                return cachedOptions;
            }

            _options.Add(options);

            return options;
        }

        private bool TryGet(string dbContextId, out IMongoDbContextOptions options)
        {
            options = _options.Where(options => options is not null)
                              .Where(options => string.Equals(options.DbContextId, dbContextId, StringComparison.OrdinalIgnoreCase))
                              .SingleOrDefault();

            return options is not null;
        }
    }
}
