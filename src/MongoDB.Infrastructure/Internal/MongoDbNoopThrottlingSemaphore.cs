using System;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbNoopThrottlingSemaphore : IMongoDbThrottlingSemaphore
    {
        private static readonly Lazy<MongoDbNoopThrottlingSemaphore> _factory = new(() =>
            new MongoDbNoopThrottlingSemaphore(), isThreadSafe: true);

        public static MongoDbNoopThrottlingSemaphore Instance => _factory.Value;

        public void AddRequest(Action request)
            => request.Invoke();

        public T AddRequest<T>(Func<T> request)
            => request.Invoke();

        public Task AddRequestAsync(Task request)
            => request;

        public Task<T> AddRequestAsync<T>(Task<T> request)
            => request;
    }
}
