using Emilie.Core.Common;
using System;
using Windows.UI.Xaml;

namespace Emilie.UWP.Common
{
    public class UWPDispatcherTimer : DispatcherTimer, IDispatcherTimer
    {
        public UWPDispatcherTimer() : base() {}

        event EventHandler<object> IDispatcherTimer.OnTick
        {
            add { Tick += value; }
            remove { Tick -= value; }
        }
    }
}
