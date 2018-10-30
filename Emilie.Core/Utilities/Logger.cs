using Emilie.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Emilie.Core
{
    /// <summary>
    /// Writes log information to the debug window.
    /// By implementing <see cref="ILogger"/> and registering the implementation
    /// using ResgisterLogger(myILogger), you can also use additional logging
    /// implementations automatically. 
    /// </summary>
    public class Logger
    {
        private static List<ILogger> _loggerPool = new List<ILogger>();

        static Logger()
        {
#if DEBUG
            RegisterLogger(new DebugConsoleLogger());
#endif
        }

        public static void RegisterLogger(ILogger logger)
        {
            _loggerPool.Add(logger);
        }

        public static bool RemoveLogger(ILogger logger)
        {
            return _loggerPool.Remove(logger);
        }

        public static bool RemoveLogger(Type t)
        {
            if (_loggerPool.FirstOrDefault(l => l.GetType() == t) is ILogger logger)
                return RemoveLogger(logger);

            return false;
        }

        public static void Log(Exception exception, [CallerMemberName] string caller = null)
        {
            foreach (var logger in _loggerPool)
                logger.Log(exception, caller);
        }

        public static void Log(String message)
        {
            foreach (var logger in _loggerPool)
                logger.Log(message);
        }

        public static void Log(String message, String title)
        {
            foreach (var logger in _loggerPool)
                logger.Log(title, message);
        }

        /// <summary>
        /// Only logs during debug.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caller"></param>
        public static void DebugLog(String message, [CallerMemberName] string caller = null)
        {
#if DEBUG
            Log(message, caller);
#endif
        }

        public static void Flush()
        {
            Task.Run(async () =>
            {
                foreach (ILogger logger in _loggerPool)
                {
                    await logger.FlushAsync().ConfigureAwait(false);
                }
            }).Wait();
        }
    }
}
