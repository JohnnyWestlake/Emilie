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
    /// Register an item to be tracked for memory leaks
    /// </summary>
    public static class LeakTrackingService
    {
        static Dictionary<String, WeakReference> _pool = new Dictionary<string, WeakReference>();

        static LeakTrackerHelper helper = new LeakTrackerHelper();

        static bool _isActive = false;

        static LeakTrackingService()
        {
            TryActivate();
        }

        static void TryActivate()
        {
            if (_isActive)
                return;

            if (Debugger.IsAttached)
            {
                DispatcherTimerHelper.RepeatEvery(TimeSpan.FromSeconds(10), CheckForLeaks);
                _isActive = true;
                // Workaround to allow us to get Media player plugins registered
                //Messenger.Default.Register<object>(helper, "LeakTrace", true, source => helper.Register(source));
            }
        }

        static long objectid = 0;
        static ulong prevMem = 0;

        /// <summary>
        /// This method is automatically called every 10 seconds. 
        /// Only call manually if you have an actual purpose for doing so.
        /// </summary>
        public static void CheckForLeaks()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("\n\n*********************************");
            sb.AppendLine("*********************************");
            sb.AppendLine("*** START LEAK TRACKING ********\n\n");

            //GC.Collect();
            AppMemoryDiagnosticReport report = CoreIoC.Get<IAppMemoryDiagnosticProvider>().GetMemoryDiagnosticReport();


            foreach (KeyValuePair<string, WeakReference> entry in _pool.ToList())
            {
                bool isAlive = entry.Value.IsAlive || entry.Value.Target != null;
                if (!isAlive)
                    _pool.Remove(entry.Key);

                sb.AppendLine(String.Format("{0} : {1}", entry.Key, isAlive ? "Alive" : "Collected"));
            }

            sb.AppendLine($"\nMemory Delta: {((long)report.AppPrivateWorkingSetUsage - (long)prevMem) / (1024 * 1024)} MB");
            sb.AppendLine($"Current Private Working Set usage: {(report.AppPrivateWorkingSetUsage / 1024) / 1024} MB out of {(report.AppMemoryUsageLimit / 1024) / 1024} MB");
            sb.AppendLine("*** END LEAK TRACKING ********\n\n");
            sb.AppendLine("*********************************");
            sb.AppendLine("*********************************");
            sb.AppendLine();

            Logger.Log(sb.ToString());

            prevMem = report.AppPrivateWorkingSetUsage;
        }

        static Random rand = new Random();

        public static void Register(object obj)
        {
            string key = obj.GetType().Name;
            string actualKey = string.Empty;

            do
            {
                objectid++;
                if (objectid > 99999)
                    objectid = 0;
                long id = objectid;

                actualKey = String.Format("{0}-{1:00000}", key, id);
            }
            while (_pool.ContainsKey(actualKey));

            _pool.Add(actualKey, new WeakReference(obj));

            Logger.Log(String.Format("LeakTrackingService: Added {0} as {1}", key, actualKey));

            TryActivate();
        }
    }
}
