using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal class ThrottlingMongoDbSemaphore : SemaphoreSlim, IThrottlingMongoDbSemaphore
    {
        public ThrottlingMongoDbSemaphore(int initialCount)
            : base(initialCount)
        { }

        public ThrottlingMongoDbSemaphore(int initialCount, int maximumCount)
            : base(initialCount, maximumCount)
        { }

        public async Task AddRequestAsync(Task task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            try
            {
                await WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
                await task.ConfigureAwait(continueOnCapturedContext: false);
            }
            finally
            {
                Release();
            }
        }

        public async Task<T> AddRequestAsync<T>(Task<T> task)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            T result;

            try
            {
                await WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
                result = await task.ConfigureAwait(continueOnCapturedContext: false);
            }
            finally
            {
                Release();
            }

            return result;
        }
    }
}
