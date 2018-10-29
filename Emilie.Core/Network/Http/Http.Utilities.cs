using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emilie.Core.Extensions;
using Emilie.Core.Serialization;

namespace Emilie.Core.Network
{
    public static partial class Http
    {
        /// <summary>
        /// Update this with necessary media types as we use them
        /// </summary>
        private static HashSet<string> KNOWN_MEDIA_TYPES = new HashSet<string>
        {
            MediaTypes.ApplicationJson,
            MediaTypes.ApplicationJsonVnd,
            MediaTypes.TextXML,
            MediaTypes.Text
        };




        //------------------------------------------------------
        //
        // Send
        //
        //------------------------------------------------------

        #region SendInternal

        static Task SendInternalAsync(
            string Uri,
            CoreHttpMethod method,
            HttpOptions options,
            ICoreHttpClient client,
            HttpResult<byte[]> result,
            CancellationToken token = default)
        {
            CoreHttpRequest request = new CoreHttpRequest
            {
                Method = method,
                Uri = Uri,
                Options = options,
            };

            return SendInternalAsync(request, client, result, token);
        }

        static Task SendInternalAsync(
            string Uri,
            CoreHttpMethod method,
            byte[] content,
            HttpOptions options,
            ICoreHttpClient client,
            HttpResult<byte[]> result,
            CancellationToken token = default)
        {
            CoreHttpRequest request = new CoreHttpRequest
            {
                Method = method,
                Uri = Uri,
                Options = options,
                Content = content
            };

            return SendInternalAsync(request, client, result, token);
        }

        static async Task SendInternalAsync(
            CoreHttpRequest request,
            ICoreHttpClient client,
            HttpResult<byte[]> result,
            CancellationToken token = default)
        {
            // 3.0.1. Configure timeout settings for the request
            TimeSpan timeout = request.Options.RequestTimeout ?? TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS);
            var timeoutSource = new CancellationTokenSource(timeout);
            CancellationTokenSource cts
                = token == default
                ? timeoutSource
                : CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, token);

            CoreHttpResponse response = await client.SendAsync(request, cts.Token).ConfigureAwait(false);
            result.StatusCode = response.StatusCode;
            result.ResponseHeaders = response.ResponseHeaders;
            result.ContentHeaders = response.ContentHeaders;
            if (response.Exception == null)
            {
                result.Content = response.Content;
                result.Success = response.IsSuccessStatusCode;
                result.FromCache = false;
            }
            else
            {
                Logger.Log(response.Exception);
                result.Exception = response.Exception;
            }
        }

        #endregion




        //------------------------------------------------------
        //
        // Parse
        //
        //------------------------------------------------------

        internal static Task<HttpResult<T>> ParseAsync<T>(HttpResult<byte[]> byteResult, ISerializer serializer)
        {
            return Task.Run(() =>
            {
                // 1. Create the shell return object based on the downloaded data result
                HttpResult<T> result = HttpResult<T>.CreateBasedOn<T>(byteResult);

                if (serializer == null)
                    serializer = Json.Default;

                if (byteResult.Content != null)
                {
                    try
                    {
                        if (typeof(T) == typeof(string))
                        {
                            result.Content = (T)(object)byteResult.Content.AsString();
                        }
                        else if (typeof(T) == typeof(byte[]))
                        {
                            result.Content = (T)(object)byteResult.Content;
                        }
                        else
                        {
                            using (Stream stream = byteResult.Content.AsStream())
                            {
                                // 2. Attempt to parse it
                                result.Content = serializer.Deserialize<T>(stream);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        result.Success = false;
                        result.Exception = ex;
                    }
                }

                return result;
            });
        }




        //------------------------------------------------------
        //
        //  FormUrlContent Encoding
        //
        //------------------------------------------------------
        /* Based on source code from .NET Core */

        private static byte[] GetFormUrlContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException(nameof(nameValueCollection));
            }

            // Encoding helper 
            string Encode(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    return string.Empty;
                }
                // Escape spaces as '+'.
                return Uri.EscapeDataString(data).Replace("%20", "+");
            }

            // Encode and concatenate data
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in nameValueCollection)
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }

                builder.Append(Encode(pair.Key));
                builder.Append('=');
                builder.Append(Encode(pair.Value));
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }
    }
}
