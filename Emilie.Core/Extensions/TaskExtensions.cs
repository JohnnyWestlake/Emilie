using System;
using System.Threading;
using System.Threading.Tasks;

namespace Emilie.Core.Extensions
{
    public static class TaskExtensions
    {
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
    }
}
