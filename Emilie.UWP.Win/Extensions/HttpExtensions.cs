using System.Collections.Generic;
using System.Linq;
using Windows.Web.Http;

namespace Emilie.UWP
{
    public static class HttpExtensions
    {
        public static HttpRequestMessage Clone(this HttpRequestMessage request)
        {
            HttpRequestMessage clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content
            };

            foreach (KeyValuePair<string, object> prop in request.Properties.ToList())
                clone.Properties.Add(prop);

            foreach (KeyValuePair<string, string> header in request.Headers.ToList())
                clone.Headers.Add(header.Key, header.Value);

            return clone;
        }
    }
}
