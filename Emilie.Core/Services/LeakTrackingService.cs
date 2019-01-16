using Emilie.Core.Common;
using Emilie.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Emilie.Core.Services
{
    public class LeakTrackerHelper
    {
        public void Register(Object obj)
        {
            LeakTrackingService.Register(obj);
        }
    }

    /// <summary>
    /// Register an item to be tracked for memory leaks.
    /// Leak tracking only occurs is the Debugger is attached.
    /// </summary>
    public static class LeakTrackingService
    {
        static List<(string, WeakReference)> _pool { get; } = new List<(string, WeakReference)>();

        static LeakTrackerHelper _helper { get; } = new LeakTrackerHelper();

        static bool _isActive = false;
        static bool _isEnabled = false;

        static LeakTrackingService()
        {
            TryActivate();
        }

        public static void Enable()
        {
            if (!_isEnabled)
            {
                _isEnabled = true;
                TryActivate();
            }
        }

        static void TryActivate()
        {
            if (!_isEnabled || _isActive)
                return;

            if (Debugger.IsAttached)
            {
                DispatcherTimerHelper.RepeatEvery(TimeSpan.FromSeconds(10), CheckForLeaks);
                _isActive = true;
                // Workaround to allow us to get Media player plugins registered
                //Messenger.Default.Register<object>(_helper, "LeakTrace", true, source => helper.Register(source));
            }
        }

        static long objectid = 0;
        static ulong prevMem = 0;

        static StringBuilder _sb = null;

        static StringBuilder GetStringBuilder()
        {
            if (_sb == null)
                _sb = new StringBuilder();
            else
                _sb.Clear();

            return _sb;
        }

        /// <summary>
        /// This method is automatically called every 10 seconds. 
        /// Only call manually if you have an actual purpose for doing so.
        /// </summary>
        public static void CheckForLeaks()
        {
            if (!Debugger.IsAttached)
                return;

            StringBuilder sb = GetStringBuilder();

            sb.AppendLine();
            sb.AppendLine("\n\n*********************************");
            sb.AppendLine("*********************************");
            sb.AppendLine("****** START LEAK TRACKING ******\n\n");

            GC.Collect();
            AppMemoryDiagnosticReport report = CoreIoC.Get<IAppMemoryDiagnosticProvider>().GetMemoryDiagnosticReport();

            foreach ((string key, WeakReference reference) entry in _pool.ToList())
            {
                var weakRef = entry.reference;
                bool isAlive = weakRef.IsAlive || weakRef.Target != null;
                if (!isAlive)
                    _pool.Remove(entry);

                sb.AppendLine(String.Format("{0} : {1}", entry.key, isAlive ? "Alive" : "Collected"));
            }

            sb.AppendLine($"\nMemory Delta: {((long)report.AppPrivateWorkingSetUsage - (long)prevMem) / (1024 * 1024)} MB");
            sb.AppendLine($"Current Private Working Set usage: {(report.AppPrivateWorkingSetUsage / 1024) / 1024} MB out of {(report.AppMemoryUsageLimit / 1024) / 1024} MB");
            sb.AppendLine("******* END LEAK TRACKING *******\n\n");
            sb.AppendLine("*********************************");
            sb.AppendLine("*********************************");
            sb.AppendLine();

            Logger.Log(sb.ToString());
            sb.Clear();

            prevMem = report.AppPrivateWorkingSetUsage;
        }

        static Random _rand { get; } = new Random();

        public static void Register(object obj)
        {
            if (!Debugger.IsAttached || obj == null)
                return;

            string key = obj.GetType().Name;
            string actualKey = string.Empty;

            objectid++;
            if (objectid > Int32.MaxValue)
                objectid = 0;
            long id = objectid;

            actualKey = String.Format("{0}-{1}", id, key);

            _pool.Add((actualKey, new WeakReference(obj)));

            Logger.Log(String.Format("LeakTrackingService: Added {0} as {1}", key, actualKey));

            TryActivate();
        }
    }
}