using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Collections.Extensions;

namespace Emilie.Core.Network
{
    public static class HttpHelpers
    {
        internal static HttpMethod AsNet(this CoreHttpMethod method)
        {
            return new HttpMethod(method.ToString());
        }

        internal static MultiValueDictionary<string, string> AsDictionary(this HttpHeaders headers)
        {
            return new MultiValueDictionary<string, string>(headers, null);
        }

        internal static void Merge(this HttpHeaders headers, MultiValueDictionary<string, string> optionHeaders)
        {
            if (optionHeaders == null)
                return;

            foreach (var value in optionHeaders)
            {
                headers.TryAddWithoutValidation(value.Key, value.Value);
            }
        }

        internal static void Merge(this IDictionary<string, object> properties, IDictionary<string, object> RequestProperties)
        {
            if (RequestProperties == null)
                return;

            foreach (var prop in RequestProperties)
            {
                properties[prop.Key] = prop.Value;
            }
        }


        /// <summary>
        /// Attempts to get the underlying <see cref="HttpBaseProtocolFilter"/> of an <see cref="IHttpFilter"/>.
        /// If the input <paramref name="filter"/> is null, a new <see cref="HttpBaseProtocolFilter"/> 
        /// is created.
        /// </summary>
        private static bool TryGetBaseHandler(ref HttpMessageHandler handler, out HttpClientHandler baseHandler)
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
            //else if (filter is HttpFilter hFilter && hFilter.InnerFilter is HttpBaseProtocolFilter b1)
            //    baseHandler = b1;
            //else if (filter is HttpBaseProtocolFilter httpBaseProtocolFilter)
            //    baseHandler = httpBaseProtocolFilter;
            else
                baseHandler = null;

            return baseHandler != null;
        }

        /// <summary>
        /// Create a new HttpClient with optimal caching settings and gzip / deflate support
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateOptimisedClient(HttpMessageHandler handler = null)
        {
            // We can optimize caching even further by using both our local cache AND WinINET's
            // cache in tandem with each other. This also enforces cache busting
            TryGetBaseHandler(ref handler, out HttpClientHandler baseHandler);

            HttpClient client = new HttpClient(handler);
            client.EnableGzipDeflate(baseHandler);
            return client;
        }

        /// <summary>
        /// Returns a new HttpClient that does not write to the WinINET cache, and has Gzip/Deflate supports,
        /// and cache-control header set to prevent caching as 'private, max-age=0, no-cache'
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateNonCachingClient(HttpMessageHandler handler = null)
        {
            TryGetBaseHandler(ref handler, out HttpClientHandler baseHandler);

            HttpClient client = new HttpClient(handler);
            client.EnableGzipDeflate(baseHandler);
            client.DefaultRequestHeaders.CacheControl.Private = true;
            client.DefaultRequestHeaders.CacheControl.MaxAge = TimeSpan.Zero;
            client.DefaultRequestHeaders.CacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl.NoStore = true;
            return client;
        }
        
        public static HttpClient NonCachingClient => CreateNonCachingClient();
        
        public static HttpClient OptimisedClient => CreateOptimisedClient();

        /// <summary>
        /// Attempts to add gzip and deflate entries to the default AcceptEncoding header
        /// </summary>
        /// <param name="client"></param>
        public static void EnableGzipDeflate(this HttpClient client, HttpClientHandler handler)
        {
            client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip");
            client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("vary", "Accept-Encoding");

            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
        }
    }
}
