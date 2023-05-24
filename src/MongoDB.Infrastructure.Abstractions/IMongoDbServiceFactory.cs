using System.Collections.Generic;

namespace MongoDB.Infrastructure
{
    public interface IMongoDbServiceFactory
    {
        T GetService<T>() where T : class;
        IEnumerable<T> GetServices<T>() where T : class;
    }
}
