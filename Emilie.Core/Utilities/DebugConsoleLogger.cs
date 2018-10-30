using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Emilie.Core.Utilities
{
    public class DebugConsoleLogger : ILogger
    {
        public void DebugLog(string message, string callerMemberName = null)
        {
            if (Debugger.IsAttached)
            {
                Log(message, callerMemberName);
            }
        }

        public void Log(Exception exception, string callerMemberName = null)
        {
            if (Debugger.IsAttached)
            {
                string ex;
                try
                {
                    ex = exception.ToString();
                }
                catch (Exception eex)
                {
                    ex = $"[VOLATILE EXCEPTION] {eex.Message}";
                }

                Debug.WriteLine(
                    "{0}: {1}", DateTime.UtcNow.ToString(), ex);
            }
        }

        public void Log(string message)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(
                    "{0}: {1}", DateTime.UtcNow.ToString(), message);
            }
        }

        public void Log(string message, string title)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(
                    "{0}: {2} - {1}", DateTime.UtcNow.ToString(), message, title);
            }
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }
    }
}
