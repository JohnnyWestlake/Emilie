using System;
using System.Threading.Tasks;

namespace Emilie.Core.Common
{
    public enum DispatcherPriority
    {
        Normal,
        Low,
        High,
        Idle
    }

    public interface IDispatcher
    {
        /// <summary>
        /// Marshalls an action to run on the UI thread if not on it.
        /// If already on UI thread, action is run straight away normally.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        Task<int> MarshallAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal);

        /// <summary>
        /// Marshalls a function to run on the UI thread if not on it.
        /// If already on UI thread, function is run straight away normally.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        Task<int> MarshallFuncAsync(Func<Task> action, DispatcherPriority priority = DispatcherPriority.Normal);

        bool HasThreadAccess { get; }
    }
}
