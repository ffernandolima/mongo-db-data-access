using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbCluster
    {
        private readonly IEnumerable<MongoServerAddress> _serverAddresses;

        public MongoDbCluster(IEnumerable<MongoServerAddress> serverAddresses)
        {
            _serverAddresses = serverAddresses ?? throw new ArgumentNullException(nameof(serverAddresses));
        }

        public static implicit operator string(MongoDbCluster cluster)
        {
            var serverAddresses = cluster.ToString();

            return serverAddresses;
        }

        public override string ToString()
        {
            var serverAddresses = string.Join(", ", _serverAddresses.Select(serverAddress => serverAddress.ToString()));

            return serverAddresses;
        }
    }
}
