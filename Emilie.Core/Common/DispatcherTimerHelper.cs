using System;

namespace Emilie.Core.Common
{
    /// <summary>
    /// Helper class for DispatcherTimer. 
    /// </summary>
    public static class DispatcherTimerHelper
    {
        /// <summary>
        /// Start a Timer and invoke an action.
        /// </summary>
        /// <param name="fromSeconds">Amount of second to wait for the timer to elapse.</param>
        /// <param name="action">Action to invoke once the amount of time has elapsed.</param>
        /// <returns>Timer that invokes action in specified time.</returns>
        public static IDispatcherTimer On(TimeSpan fromSeconds, Action action)
        {

            IDispatcherTimer t = CoreIoC.New<IDispatcherTimer>();
            t.Interval = fromSeconds;
            EventHandler<object> tick = null;
            tick = new EventHandler<object>((s, args) =>
            {
                action();
                t.Stop();
                t.OnTick -= tick;
                tick = null;
                t = null;
            });

            t.OnTick += tick;
            t.Start();

            return t;
        }

        /// <summary>
        /// Start a Timer and invoke an action.
        /// </summary>
        /// <param name="fromSeconds">Amount of second to wait for the timer to elapse.</param>
        /// <param name="action">Action to invoke once the amount of time has elapsed.</param>
        /// <returns>Timer that invokes action in specified time.</returns>
        public static void OnUi(TimeSpan fromSeconds, Action action)
        {
            DispatcherHelper.MarshallAsync(() =>
            {
                On(fromSeconds, action);
            });
        }

        /// <summary>
        /// Start a Timer and invoke an action.
        /// </summary>
        /// <param name="fromSeconds">
        /// Amount of second to wait for the timer to elapse.
        /// </param>
        /// <param name="action">
        /// Action to invoke once the amount of time has elapsed.
        /// </param>
        /// <param name="actionArgs">
        /// The action Args.
        /// </param>
        public static void On(TimeSpan fromSeconds, Delegate action, object[] actionArgs)
        {
            IDispatcherTimer t = CoreIoC.New<IDispatcherTimer>();
            t.Interval = fromSeconds;
            t.OnTick += (s, args) =>
            {
                action.DynamicInvoke(actionArgs);
                t.Stop();
            };
            t.Start();
        }

        /// <summary>
        /// Start a Timer and repeat an action
        /// </summary>
        /// <param name="fromSeconds">Amount of seconds to wait between invoking the action</param>
        /// <param name="action">Action to invoke once the interval has elapsed.</param>
        /// <returns>Returns a reference to the DispatcherTimer</returns>
        public static IDispatcherTimer RepeatEvery(TimeSpan fromSeconds, Action action)
        {
            IDispatcherTimer t = CoreIoC.New<IDispatcherTimer>();
            t.Interval = fromSeconds;

            t.OnTick += (s, args) =>
            {
                action();
            };

            t.Start();

            return t;
        }

        /// <summary>
        /// Start a Timer and repeat an action
        /// </summary>
        /// <param name="fromSeconds">Amount of seconds to wait between invoking the action</param>
        /// <param name="action">Action to invoke once the interval has elapsed.</param>
        /// <param name="actionArgs">The action Args.</param>
        /// <returns>Returns a reference to the DispatcherTimer</returns>
        public static IDispatcherTimer RepeatEvery(TimeSpan fromSeconds, Delegate action, object[] actionArgs)
        {
            IDispatcherTimer t = CoreIoC.New<IDispatcherTimer>();
            t.Interval = fromSeconds;

            t.OnTick += (s, args) =>
            {
                action.DynamicInvoke(actionArgs);
            };
            t.Start();

            return t;
        }
    }
}