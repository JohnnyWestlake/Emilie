using System;

namespace Emilie.Core
{
    /// <summary>
    /// Adds support for SlowLoading and uniquely identified loading sessions
    /// </summary>
    public interface ILoadableResult1
    {
        /// <summary>
        /// An ID that changes every time the loadstate is re-entered. Use for 
        /// concurrency checking
        /// </summary>
        Guid LoadingId { get; }

        Boolean IsSlowLoading { get; }
        /// <summary>
        /// Trigger the slow loading state
        /// </summary>
        void SetSlowLoading();
        /// <summary>
        /// The amount of time to wait after switching to the loading state before also triggering the "IsSlowLoading" state
        /// </summary>
        TimeSpan? SlowPeriod { get; set; }
    }
}