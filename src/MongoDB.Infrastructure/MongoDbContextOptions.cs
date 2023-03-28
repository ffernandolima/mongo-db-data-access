using MongoDB.Driver;
using System;

namespace MongoDB.Infrastructure
{
    public class MongoDbContextOptions : IMongoDbContextOptions
    {
        private readonly MongoClientSettings _settings;
        private int _maximumNumberOfConcurrentRequests;

        public bool AcceptAllChangesOnSave { get; set; } = true;
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
}
