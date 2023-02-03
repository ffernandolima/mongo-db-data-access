using System;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal class NoopThrottlingMongoDbSemaphore : IThrottlingMongoDbSemaphore
    {
        private static readonly Lazy<NoopThrottlingMongoDbSemaphore> Factory = new(() =>
            new NoopThrottlingMongoDbSemaphore(), isThreadSafe: true);

        public static NoopThrottlingMongoDbSemaphore Instance => Factory.Value;

        public Task AddRequestAsync(Task task) => task;

        public Task<T> AddRequestAsync<T>(Task<T> task) => task;
    }
}
