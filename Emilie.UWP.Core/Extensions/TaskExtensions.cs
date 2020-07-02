using Emilie.Core;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace Emilie.UWP
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable<T> ConfigureAwait<T>(this IAsyncOperation<T> operation, bool continueOnCapturedContext = false)
        {
            return operation.AsTask().ConfigureAwait(continueOnCapturedContext);
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this IAsyncAction operation, bool continueOnCapturedContext = false)
        {
            return operation.AsTask().ConfigureAwait(continueOnCapturedContext);
        }

        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
                return task.Result;
            else if (cancellationToken.IsCancellationRequested)
                return await (new Task<T>(() => default, cancellationToken));
            else
            {
                var tcs = new TaskCompletionSource<bool>();
                using (cancellationToken.Register(() => tcs.TrySetResult(true)))
                    if (task != await Task.WhenAny(task, tcs.Task))
                        throw new OperationCanceledException(cancellationToken);
                return await task;
            }
        }

        [Deprecated("Only use this method if you want to swallowed exceptions. Not recommended", DeprecationType.Remove, 1)]
        /// <summary>
        /// Allows the internal task to fail, but still allow the caller method to continue after
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task SafeRun(Action action)
        {
            TaskCompletionSource<Boolean> tcs = new TaskCompletionSource<bool>();
            Task t = Task.Run(() =>
            {
                try
                {
                    action.Invoke();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}
