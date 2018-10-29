using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Emilie.Core
{
    /// <summary>
    /// Writes log information to the debug window.
    /// By implementing <see cref="ILogger"/> and registering the implementation
    /// using ResgisterLogger(myILogger), you can also use additional logging
    /// implementations automatically. (For example, you can implement a bugsense
    /// ILogger that will send all logged exceptions to BugSense)
    /// </summary>
    public class Logger
    {
        private static List<ILogger> _loggerPool = new List<ILogger>();

        public static void RegsiterLogger(ILogger logger)
        {
            _loggerPool.Add(logger);
        }

        public static void Log(Exception exception, [CallerMemberName] string caller = null)
        {
            var s = String.Format("{0}: {1}", DateTime.Now.ToLocalTime().ToString(), exception.ToString());
            Debug.WriteLine(s);

            // Ensure any subscribed loggers also log this exception
            foreach (var logger in _loggerPool)
                logger.Log(exception, caller);
        }

        public static void Log(String message)
        {
            var s = String.Format("{0}: {1}", DateTime.Now.ToLocalTime().ToString(), message);
            Debug.WriteLine(s);

            // Ensure any subscribed loggers also log this message
            foreach (var logger in _loggerPool)
                logger.Log(message);
        }

        public static void Log(String message, String title)
        {
            var s = String.Format("{0}: {2} - {1}", DateTime.Now.ToLocalTime().ToString(), message, title);
            Debug.WriteLine(s);

            // Ensure any subscribed loggers also log this title and message
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
    }
}
