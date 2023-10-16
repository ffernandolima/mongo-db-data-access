using MongoDB.Driver;
using MongoDB.Infrastructure.Internal;
using System;

namespace MongoDB.Infrastructure
{
    public abstract class MongoDbContextOptions : IMongoDbContextOptions
    {
        private readonly MongoClientSettings _settings;
        private int _maximumNumberOfConcurrentRequests;

        public string DbContextId { get; set; } = $"{Guid.NewGuid()}";
        public bool AcceptAllChangesOnSave { get; set; } = true;
        /// <Remarks>
        /// -1 must be set to disable the requests throttling mechanism.
        /// </Remarks>
        public int MaximumNumberOfConcurrentRequests
        {
            get => _maximumNumberOfConcurrentRequests;
            set
            {
                var maximumNumberOfConcurrentRequests = value;

                _maximumNumberOfConcurrentRequests = maximumNumberOfConcurrentRequests != 0
                    ? maximumNumberOfConcurrentRequests
                    : GetDefaultMaximumNumberOfConcurrentRequests();
            }
        }

        public MongoDbContextOptions(MongoClientSettings settings = null)
        {
            _settings = settings;
            _maximumNumberOfConcurrentRequests = GetDefaultMaximumNumberOfConcurrentRequests();
        }

        private int GetDefaultMaximumNumberOfConcurrentRequests()
        {
            var maxConnectionPoolSize = _settings?.MaxConnectionPoolSize ?? MongoDefaults.MaxConnectionPoolSize;
            var maximumNumberOfConcurrentRequests = Math.Max(maxConnectionPoolSize / 2, 1);

            return maximumNumberOfConcurrentRequests;
        }
    }

    public class MongoDbContextOptions<T> : MongoDbContextOptions, IMongoDbContextOptions<T>
        where T : IMongoDbContext
    {
        public Type DbContextType => typeof(T);

        public MongoDbContextOptions(MongoClientSettings settings = null)
            : base(settings)
        {
            DbContextId = $"{DbContextType.ExtractTypeName()} - {DbContextId}";
        }
    }
}
