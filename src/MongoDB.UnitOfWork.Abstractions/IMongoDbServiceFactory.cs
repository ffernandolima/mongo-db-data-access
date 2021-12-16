using System;

namespace MongoDB.UnitOfWork
{
    public interface IMongoDbServiceFactory : IDisposable
    {
        T GetService<T>() where T : class;
    }
}
