using MongoDB.Driver;
using System;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbSession
    {
        public MongoDbSession(IClientSessionHandle sessionHandle)
        {
            SessionHandle = sessionHandle ?? throw new ArgumentNullException(nameof(sessionHandle));
        }

        public Guid Id { get; } = Guid.NewGuid();
        public IClientSessionHandle SessionHandle { get; }
    }
}
