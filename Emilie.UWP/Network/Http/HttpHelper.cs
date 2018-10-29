using Microsoft.Collections.Extensions;
using System.Collections.Generic;
using System.Linq;
using Windows.Web.Http.Headers;

namespace Emilie.UWP.Network
{
    public static class HttpHelper
    {
        public static void Merge(this HttpRequestHeaderCollection headers, MultiValueDictionary<string, string> optionHeaders)
        {
            if (optionHeaders == null)
                return;

            foreach (var header in optionHeaders)
            {
                headers.TryAppendWithoutValidation(header.Key, header.Value.First());
            }
        }

        public static void Merge(this HttpContentHeaderCollection headers, MultiValueDictionary<string, string> optionHeaders)
        {
            if (optionHeaders == null)
                return;

            foreach (var header in optionHeaders)
            {
                headers.TryAppendWithoutValidation(header.Key, header.Value.First());
            }
        }

        public static void Merge(this IDictionary<string, object> properties, IDictionary<string, object> RequestProperties)
        {
            if (RequestProperties == null)
                return;

            foreach (var prop in RequestProperties)
            {
                properties[prop.Key] = prop.Value;
            }
        }
    }
}
