using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Emilie.Core.Utilities
{
    public class FileLogger : ILogger
    {
        SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        string _path = null;

        bool _opened = false;

        public FileLogger(string path)
        {
            _path = path;
        }

        public void DebugLog(string message, string callerMemberName = null)
        {
            Log(message, callerMemberName);
        }

        public void Log(Exception exception, string callerMemberName = null)
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

            _ = WriteAsync("{0}: {1}", DateTime.UtcNow.ToString(), ex);
        }

        public void Log(string message)
        {
            _ = WriteAsync("{0}: {1}", DateTime.UtcNow.ToString(), message);
        }

        public void Log(string message, string title)
        {
            _ = WriteAsync("{0}: {2} - {1}", DateTime.UtcNow.ToString(), message, title);
        }

        Task WriteAsync(string message, params object[] args)
        {
            _semaphoreSlim.Wait();
            return Task.Run(() =>
            {
                try
                {
                    if (!_opened)
                    {
                        File.Create(_path).Dispose();
                        _opened = true;
                    }
                    File.AppendAllLines(_path, new string[] { string.Format(message, args) });
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            });
        }

        public Task FlushAsync()
        {
            return Task.Run(() =>
            {
                _semaphoreSlim.Wait();
            });
        }
    }
}
