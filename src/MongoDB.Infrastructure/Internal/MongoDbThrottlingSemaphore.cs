using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbThrottlingSemaphore : SemaphoreSlim, IMongoDbThrottlingSemaphore
    {
        public MongoDbThrottlingSemaphore(int initialCount)
            : base(initialCount)
        { }

        public MongoDbThrottlingSemaphore(int initialCount, int maximumCount)
            : base(initialCount, maximumCount)
        { }

        public void AddRequest(Action request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request), $"{nameof(request)} cannot be null.");
            }

            try
            {
                Wait();
                request.Invoke();
            }
            finally
            {
                Release();
            }
        }

        public T AddRequest<T>(Func<T> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request), $"{nameof(request)} cannot be null.");
            }

            T result;

            try
            {
                Wait();
                result = request.Invoke();
            }
            finally
            {
                Release();
            }

            return result;
        }

        public async Task AddRequestAsync(Task request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request), $"{nameof(request)} cannot be null.");
            }

            try
            {
                await WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
                await request.ConfigureAwait(continueOnCapturedContext: false);
            }
            finally
            {
                Release();
            }
        }

        public async Task<T> AddRequestAsync<T>(Task<T> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request), $"{nameof(request)} cannot be null.");
            }

            T result;

            try
            {
                await WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
                result = await request.ConfigureAwait(continueOnCapturedContext: false);
            }
            finally
            {
                Release();
            }

            return result;
        }
    }
}
