using System;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal interface IMongoDbThrottlingSemaphore
    {
        void AddRequest(Action request);
        T AddRequest<T>(Func<T> request);
        Task AddRequestAsync(Task request);
        Task<T> AddRequestAsync<T>(Task<T> request);
    }
}
