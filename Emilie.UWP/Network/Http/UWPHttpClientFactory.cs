using Emilie.Core.Network;
using Windows.Networking.Connectivity;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Emilie.UWP.Network
{
    public class UWPHttpClientFactory : ICoreHttpClientFactory
    {
        private bool _isNetworkAvailable = false;

        public UWPHttpClientFactory()
        {
            NetworkInformation.NetworkStatusChanged += NetworkStatusChanged;
            UpdateNetworkStatus();
        }

        public ICoreHttpClient CreateNonCachingCoreClient()
        {
            return new UWPHttpClient(CreateNonCachingClient());
        }

        public ICoreHttpClient CreateOptimisedCoreClient()
        {
            return new UWPHttpClient(CreateOptimisedClient());
        }

        public bool IsNetworkAvailable()
        {
            return _isNetworkAvailable;
        }

        public void Dispose()
        {
            NetworkInformation.NetworkStatusChanged -= NetworkStatusChanged;
        }


        /// <summary>
        /// Attempts to add gzip and deflate entries to the default AcceptEncoding header
        /// </summary>
        /// <param name="client"></param>
        public static void EnableGzipDeflate(HttpClient client)
        {
            client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip");
            client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("deflate");
            client.DefaultRequestHeaders.TryAppendWithoutValidation("vary", "Accept-Encoding");
        }

        /// <summary>
        /// Attempts to get the underlying <see cref="HttpBaseProtocolFilter"/> of an <see cref="IHttpFilter"/>.
        /// If the input <paramref name="filter"/> is null, a new <see cref="HttpBaseProtocolFilter"/> 
        /// is created.
        /// </summary>
        private static bool TryGetBaseFilter(ref IHttpFilter filter, out HttpBaseProtocolFilter baseFilter)
        {
            if (filter == null)
            {
                baseFilter = new HttpBaseProtocolFilter();
                filter = baseFilter;
            }
            else if (filter is HttpFilter hFilter && hFilter.InnerFilter is HttpBaseProtocolFilter b1)
                baseFilter = b1;
            else if (filter is HttpBaseProtocolFilter httpBaseProtocolFilter)
                baseFilter = httpBaseProtocolFilter;
            else
                baseFilter = null;

            return baseFilter != null;
        }

        /// <summary>
        /// Create a new HttpClient with optimal caching settings and gzip / deflate support
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateOptimisedClient(IHttpFilter filter = null)
        {
            // We can optimize caching even further by using both our local cache AND WinINET's
            // cache in tandem with each other. This also enforces cache busting
            if (TryGetBaseFilter(ref filter, out HttpBaseProtocolFilter baseFilter))
            {
                baseFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            }

            HttpClient client = new HttpClient(filter);
            EnableGzipDeflate(client);
            return client;
        }

        /// <summary>
        /// Returns a new HttpClient that does not write to the WinINET cache, and has Gzip/Deflate supports,
        /// and cache-control header set to prevent caching as 'private, max-age=0, no-cache'
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateNonCachingClient(IHttpFilter filter = null)
        {
            if (TryGetBaseFilter(ref filter, out HttpBaseProtocolFilter baseFilter))
            {
                baseFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                baseFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            }

            HttpClient client = new HttpClient(filter);
            EnableGzipDeflate(client);
            client.DefaultRequestHeaders.CacheControl.TryParseAdd("private");
            client.DefaultRequestHeaders.CacheControl.TryParseAdd("max-age=0");
            client.DefaultRequestHeaders.CacheControl.TryParseAdd("no-cache");
            return client;
        }

        private void NetworkStatusChanged(object sender)
        {
            UpdateNetworkStatus();
        }

        private void UpdateNetworkStatus()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile == null)
            {
                _isNetworkAvailable = false;
                return;
            }

            //if (profile.GetConnectionCost().ApproachingDataLimit
            //    || profile.GetConnectionCost().Roaming
            //    || profile.GetConnectionCost().OverDataLimit)
            //    IsBandwidthRestricted = true;

            var lvl = profile.GetNetworkConnectivityLevel();
            _isNetworkAvailable = (lvl == NetworkConnectivityLevel.InternetAccess);
        }
    }
}
