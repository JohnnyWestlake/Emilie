using System;
using System.Collections.Generic;

namespace Emilie.Core.Network
{
    public static partial class Http
    {
        #region Properties

        /// <summary>
        /// Default HTTP request timeout in seconds - 30 Seconds
        /// </summary>
        public const Int32 DEFAULT_TIMEOUT_SECONDS = 30;

        public static INetworkCache DefaultCache { get; set; }

        /// <summary>
        /// A list of HResult Error codes with friendlier identifiers for typical connection failed
        /// exceptions (as Windows.Web.Http doesn't bubble up nice .NET exceptions for us, it being
        /// a native WindowsRuntime COM-based component and all)
        /// </summary>
        public static Dictionary<String, String> ConnectionFailedExceptions = new Dictionary<string, string>
        {
            { "0x80072F30", "ERROR_WINHTTP_NO_CM_CONNECTION" },
            { "0x80072EFELL", "WININET_E_CONNECTION_ABORTED" },
            { "0x80072EFDL", "WININET_E_CANNOT_CONNECT" },
            { "0x80072EE7", "INVALID_IP_ADDRESS" }
        };

        #endregion

        static ICoreHttpClientFactory _clientFactory { get; }

        static Http()
        {
            _clientFactory = CoreIoC.Get<ICoreHttpClientFactory>();
        }

        private static void EnsureInitialised() { }

        public static bool IsConnectionAvailable => _clientFactory.IsNetworkAvailable();

        /// <summary>
        /// Create a new HttpClient with optimal caching settings and gzip / deflate support
        /// </summary>
        /// <returns></returns>
        public static ICoreHttpClient CreateOptimisedClient()
        {
            return _clientFactory.CreateOptimisedCoreClient();
        }

        /// <summary>
        /// Returns a new HttpClient that does not write to the WinINET cache, and has Gzip/Deflate supports,
        /// and cache-control header set to prevent caching as 'private, max-age=0, no-cache'
        /// </summary>
        /// <returns></returns>
        public static ICoreHttpClient CreateNonCachingClient()
        {
            return _clientFactory.CreateNonCachingCoreClient();
        }
        
        public static ICoreHttpClient NonCachingClient => CreateNonCachingClient();
        
        public static ICoreHttpClient OptimisedClient => CreateOptimisedClient();

        /// <summary>
        /// Returns a friendly identifier for exceptions if a known connection failure is 
        /// recognized (typically due to a dropped connection over a mobile network)
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static String GetConnectionExceptionDetails(Exception ex)
        {
            String details = null;

            foreach (var entry in ConnectionFailedExceptions)
            {
                if (ex.Message.IndexOf(entry.Key) > -1)
                {
                    details = entry.Value;
                    break;
                }
            }

            return details;
        }
    }
}
