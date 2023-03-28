using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Extensions
{
    public static class MongoDbTaskExtensions
    {
        public static Task<TResult> Then<TSource, TResult>(
            this Task<TSource> sourceTask,
            Func<TSource, TResult> selector,
            CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();

            sourceTask.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    taskCompletionSource.TrySetException(task.Exception.InnerExceptions);
                }
                else if (task.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    try
                    {
                        var result = selector.Invoke(task.Result);

                        taskCompletionSource.TrySetResult(result);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                }

            }, cancellationToken);

            return taskCompletionSource.Task;
        }

        public static Task<TResult> Then<TResult>(
            this Task sourceTask,
            Func<TResult> selector,
            CancellationToken cancellationToken = default)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();

            sourceTask.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    taskCompletionSource.TrySetException(task.Exception.InnerExceptions);
                }
                else if (task.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else
                {
                    try
                    {
                        var result = selector.Invoke();

                        taskCompletionSource.TrySetResult(result);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                }

            }, cancellationToken);

            return taskCompletionSource.Task;
        }
    }
}
