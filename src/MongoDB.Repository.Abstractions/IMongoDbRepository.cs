using System;

namespace MongoDB.Repository.Abstractions
{
    public interface IMongoDbRepository : IDisposable
    { }

    public interface IMongoDbRepository<T> : IMongoDbRepository, ISyncMongoDbRepository<T>, IAsyncMongoDbRepository<T>, IDisposable where T : class
    { }
}
