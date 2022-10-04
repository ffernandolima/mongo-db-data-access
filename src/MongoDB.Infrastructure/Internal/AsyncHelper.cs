using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new(
            CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default
        );

        public static TResult RunSync<TResult>(Func<Task<TResult>> function)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            var currentCulture = CultureInfo.CurrentCulture;
            var currentUICulture = CultureInfo.CurrentUICulture;

            Task<TResult> DecoratedFunction()
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentUICulture;

                return function.Invoke();
            }

            return TaskFactory.StartNew(DecoratedFunction)
                              .Unwrap()
                              .GetAwaiter()
                              .GetResult();
        }

        public static void RunSync(Func<Task> function)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            var currentCulture = CultureInfo.CurrentCulture;
            var currentUICulture = CultureInfo.CurrentUICulture;

            Task DecoratedFunction()
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentUICulture;

                return function.Invoke();
            }

            TaskFactory.StartNew(DecoratedFunction)
                       .Unwrap()
                       .GetAwaiter()
                       .GetResult();
        }
    }
}