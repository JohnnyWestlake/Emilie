using System;
using System.Net.Http;

namespace Emilie.Core.Network
{
    public class NetHttpFactory : ICoreHttpClientFactory
    {
        public ICoreHttpClient CreateNonCachingCoreClient()
        {
            return new NetHttpClient(CreateNonCachingClient(null));
        }

        public ICoreHttpClient CreateOptimisedCoreClient()
        {
            return new NetHttpClient(CreateOptimisedClient(null));
        }

        public static HttpClient CreateNonCachingClient(HttpMessageHandler handler = null)
        {
            TryGetBaseClientHandler(ref handler, out HttpClientHandler baseHandler);

            HttpClient client = new HttpClient(handler);
            client.EnableGzipDeflate(baseHandler);
            client.DefaultRequestHeaders.CacheControl.Private = true;
            client.DefaultRequestHeaders.CacheControl.MaxAge = TimeSpan.Zero;
            client.DefaultRequestHeaders.CacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl.NoStore = true;
            return client;
        }

        public static HttpClient CreateOptimisedClient(HttpMessageHandler handler = null)
        {
            TryGetBaseClientHandler(ref handler, out HttpClientHandler baseHandler);

            HttpClient client = new HttpClient(handler ?? baseHandler);
            client.EnableGzipDeflate(baseHandler);
            return client;
        }

        public bool IsNetworkAvailable()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        public void EnableGzipDeflate(HttpClient client, HttpClientHandler handler = null)
        {
            client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip");
            client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("vary", "Accept-Encoding");

            if (handler != null)
            {
                handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
            }
        }

        private static bool TryGetBaseClientHandler(ref HttpMessageHandler handler, out HttpClientHandler baseHandler)
        {
            if (handler == null)
            {
                baseHandler = new HttpClientHandler();
                handler = baseHandler;
            }
            else if (handler is HttpClientHandler hCh)
            {
                baseHandler = hCh;
            }
            else if (handler is DelegatingHandler dh && dh.InnerHandler is HttpClientHandler dhch)
            {
                baseHandler = dhch;
            }
            else
                baseHandler = null;

            return baseHandler != null;
        }

        public void Dispose()
        {
        }
    }
}
