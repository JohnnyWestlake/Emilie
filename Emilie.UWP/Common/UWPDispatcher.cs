using Emilie.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Emilie.UWP.Common
{
    //[QualityBand(QualityBand.Preview)]
    public class UWPDispatcher : IDispatcher
    {
        private static CoreDispatcher _dispatcher;

        /// <summary>
        /// Returns the UI thread Dispatcher for the Core Window of the application
        /// </summary>
        public static CoreDispatcher Dispatcher => _dispatcher ?? (_dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher);

        static CoreDispatcherPriority AsCorePriority(DispatcherPriority priority)
        {
            switch (priority)
            {
                case DispatcherPriority.Normal:
                    return CoreDispatcherPriority.Normal;
                case DispatcherPriority.Low:
                    return CoreDispatcherPriority.Low;
                case DispatcherPriority.High:
                    return CoreDispatcherPriority.High;
                case DispatcherPriority.Idle:
                    return CoreDispatcherPriority.Idle;
                default:
                    return CoreDispatcherPriority.Normal;
            }
        }

        public bool HasThreadAccess => Dispatcher.HasThreadAccess;

        /// <summary>
        /// Schedules an Action to run on the UI thread if we are not already on the UI thread. If we already on the UI thread, we
        /// run the action immediately.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public Task<int> MarshallAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var tcs = new TaskCompletionSource<int>();

            try
            {
                if (Dispatcher.HasThreadAccess)
                {
                    action.Invoke();
                    tcs.SetResult(0);
                }
                else
                {
                    var a = Dispatcher.RunAsync(AsCorePriority(priority), () =>
                    {
                        action.Invoke();
                        tcs.SetResult(0);
                    });
                }
                    
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }


        /// <summary>
        /// Ensures an action is ALWAYS run on the UI thread. If there is no UI thread, things will die.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public Task<int> MarshallFuncAsync(Func<Task> action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var tcs = new TaskCompletionSource<int>();

            try
            {
                if (Dispatcher.HasThreadAccess)
                {
                    (action.Invoke()).ContinueWith((t) => tcs.SetResult(0), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
                }
                else
                {
                    var a = Dispatcher.RunAsync(AsCorePriority(priority), async () =>
                    {
                        await action.Invoke();
                        tcs.SetResult(0);
                    });
                }
                    
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }
    }


}