using System;
using System.Threading.Tasks;

namespace Emilie.Core.Common
{
    //[QualityBand(QualityBand.Preview)]
    public class DispatcherHelper
    {
        private static IDispatcher _dispatcher;

        /// <summary>
        /// Returns the UI thread Dispatcher for the Core Window of the application
        /// </summary>
        public static IDispatcher Dispatcher => _dispatcher ?? (_dispatcher = CoreIoC.Get<IDispatcher>());



        /// <summary>
        /// Schedules an Action to run on the UI thread if we are not already on the UI thread. If we already on the UI thread, we
        /// run the action immediately.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static Task<int> MarshallAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return Dispatcher.MarshallAsync(action, priority);
        }


        /// <summary>
        /// Ensures an action is ALWAYS run on the UI thread. If there is no UI thread, things will die.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public static Task<int> MarshallFuncAsync(Func<Task> action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return Dispatcher.MarshallFuncAsync(action, priority);
        }
    }
}