using Emilie.Core.Common;
using System;

namespace Emilie.Core
{
    public static class SlowLoadingService
    {

        static readonly TimeSpan DEFAULT_PERIOD = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Allows us to easily, automatically mark an item as SlowLoading
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="period"></param>
        public static void StartTracking<T>(T item) where T : ILoadableResult, ILoadableResult1
        {
            TimeSpan? period = item.SlowPeriod;
            TimeSpan wait = period != null && period.HasValue ? period.Value : DEFAULT_PERIOD;

            WeakReference wref = new WeakReference(item);
            Guid originalId = item.LoadingId;

            DispatcherTimerHelper.OnUi(wait, () =>
            {
                try
                {
                    if (!wref.IsAlive)
                        return;

                    ILoadableResult1 i = wref.Target as ILoadableResult1;
                    ILoadableResult loadable = i as ILoadableResult;

                    if (i == null || loadable == null)
                        return;

                    if (loadable.IsLoading && i.LoadingId == originalId)
                        i.SetSlowLoading();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }
    }
}