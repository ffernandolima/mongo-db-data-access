using System;

namespace MongoDB.UnitOfWork.Abstractions
{
    public interface IMongoDbServiceFactory : IDisposable
    {
        T GetService<T>() where T : class;
    }
}
