using System;

namespace Emilie.Core.Common
{
    public interface IDispatcherTimer
    {
        void Start();
        void Stop();
        TimeSpan Interval { get; set; }
        bool IsEnabled { get; }
        event EventHandler<object> OnTick;
    }
}