using System;

namespace Emilie.Core
{
    public interface ILogger
    {
        void Log(Exception exception, String callerMemberName = null);
        void Log(String message);
        void Log(String message, String title);
        void DebugLog(String message, String callerMemberName = null);
    }
}
