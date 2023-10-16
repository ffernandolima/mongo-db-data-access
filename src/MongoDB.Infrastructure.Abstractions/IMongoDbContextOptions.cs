using System;

namespace MongoDB.Infrastructure
{
    public interface IMongoDbContextOptions
    {
        string DbContextId { get; }
        bool AcceptAllChangesOnSave { get; }
        int MaximumNumberOfConcurrentRequests { get; }
    }

    public interface IMongoDbContextOptions<T> : IMongoDbContextOptions
         where T : IMongoDbContext
    {
        Type DbContextType { get; }
    }
}
