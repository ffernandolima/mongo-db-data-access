using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal class ThrottlingMongoDbSemaphore : SemaphoreSlim
    {
        public ThrottlingMongoDbSemaphore(int initialCount)
            : base(initialCount)
        { }

        public ThrottlingMongoDbSemaphore(int initialCount, int maximumCount)
            : base(initialCount, maximumCount)
        { }

        public async Task<T> AddRequest<T>(Task<T> task)
        {
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

        public async Task AddRequest(Task task)
        {
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
    }
}
