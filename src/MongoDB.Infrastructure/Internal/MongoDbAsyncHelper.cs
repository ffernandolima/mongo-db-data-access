using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoDbAsyncHelper
    {
        private static readonly TaskFactory _taskFactory = new(
            CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default
        );

        public static TResult RunSync<TResult>(Func<Task<TResult>> function)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function), $"{nameof(function)} cannot be null.");
            }

            var currentCulture = CultureInfo.CurrentCulture;
            var currentUICulture = CultureInfo.CurrentUICulture;

            return _taskFactory.StartNew(() =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUICulture;

                    return function.Invoke();
                })
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(Func<Task> function)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function), $"{nameof(function)} cannot be null.");
            }

            var currentCulture = CultureInfo.CurrentCulture;
            var currentUICulture = CultureInfo.CurrentUICulture;

            _taskFactory.StartNew(() =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUICulture;

                    return function.Invoke();
                })
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}