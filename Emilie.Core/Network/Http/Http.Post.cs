using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emilie.Core.Serialization;

namespace Emilie.Core.Network
{
    public static partial class Http
    {
        /* 
        /*
        /* This file contains all the methods related to HttpPost requests 
        /*
        */

        [QualityBand(QualityBand.ActiveDevelopment)]
        /// <summary>
        /// Base Http POST method. Ignores response data from the server, and only
        /// checks for HttpStatusCode == Success
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="content"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Task<HttpResult> PostWithNoResultAsync(
            string endpoint,
            byte[] content,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                // 1. Create our response wrapper object.
                //    Note, not all POST requests return data, so you may have to create another 
                //    method to deal with this
                HttpResult<byte[]> result = new HttpResult<byte[]>
                {
                    OriginalUri = endpoint,
                    WasConnectionAvaliable = IsConnectionAvailable,
                    Success = false,
                    FromCache = false
                };

                // 2. Only attempt the post if the server connection exists
                if (result.WasConnectionAvaliable)
                {
                    ICoreHttpClient internalClient = null;
                    try
                    {
                        // 3. Set the HttpClient we'll be using. If one isn't passed in
                        //    as an override, we'll create a new non-caching client
                        internalClient = client ?? Http.NonCachingClient;
                        await SendInternalAsync(endpoint, CoreHttpMethod.Post, content, options, client, result, token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        result.Exception = ex;
                    }
                    finally
                    {
                        
                    }

                }

                return HttpResult.CreateBasicCopy(result);
            });
        }

        /// <summary>
        /// Base Http POST method. Returns a parsed response from the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="content"></param>
        /// <param name="serializer"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Task<HttpResult<T>> PostContentAsync<T>(
            string endpoint,
            byte[] content,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                // 1. Create our response wrapper object.
                //    Note, not all POST requests return data, so you may have to create another 
                //    method to deal with this
                HttpResult<byte[]> byteResult = new HttpResult<byte[]>
                {
                    OriginalUri = endpoint,
                    WasConnectionAvaliable = IsConnectionAvailable,
                    Success = false,
                    FromCache = false
                };

                // 2. Only attempt the post if the server connection exists
                if (IsConnectionAvailable)
                {
                    try
                    {
                        // 3. Set the HttpClient we'll be using. If one isn't passed in
                        //    as an override, we'll create a new non-caching client
                        ICoreHttpClient internalClient = client ?? Http.NonCachingClient;
                        await SendInternalAsync(endpoint, CoreHttpMethod.Post, content, options, client, byteResult, token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        byteResult.Exception = ex;
                    }
                    finally
                    {
                    }
                }

                return await ParseAsync<T>(byteResult, options.Serializer);
            });
        }

        public static Task<HttpResult<TResult>> PostAsync<TResult>(
           Endpoint<TResult> endpoint,
           object data,
           string mediaType = null,
           ICoreHttpClient client = null,
           CancellationToken token = default)
        {
            return PostAsync<TResult>(
                endpoint.Uri.OriginalString,
                data,
                mediaType,
                endpoint,
                client,
                token);
        }

       [QualityBand(QualityBand.Experimental, "No error handling during serialization phase")]
        /// <summary>
        /// Sends an object as UTF-8 encoded Raw data over a POST request
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <param name="serializer">Override serializer used to serialize the object and deserialize the response. If not provided, the default Newtonsoft Json Serialiser is used</param>
        /// <param name="client">Override client. If not provided, a default non-caching client is used</param>
        /// <returns></returns>
        public static Task<HttpResult<T>> PostAsync<T>(
           string endpoint,
           object data,
           string mediaType = null,
           HttpOptions options = null,
           ICoreHttpClient client = null,
           CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                options = options ?? new HttpOptions();

                // 1. Ensure we have a serializer
                if (options.Serializer == null)
                    options.Serializer = Json.Default;

                if (mediaType != null)
                {
                    if (options.ContentHeaders == null)
                        options.ContentHeaders = new HeaderCollection();
                    options.ContentHeaders.Set("Content-Type", MediaTypes.ApplicationJson);
                }

                if (data == null)
                {
                    return await PostContentAsync<T>(endpoint, null, options, client, token).ConfigureAwait(false);
                }
                else if (data is string stringData)
                {
                    return await PostContentAsync<T>(
                            endpoint, Encoding.UTF8.GetBytes(stringData), options, client, token).ConfigureAwait(false);
                }
                else
                {
                    // 4.1. We're going to send as stream content here, so let's get our bytes
                    if (options.ContentHeaders == null)
                        options.ContentHeaders = new HeaderCollection();
                    options.ContentHeaders.Set("Content-Type", MediaTypes.ApplicationJson);

                    byte[] encodedData = Encoding.UTF8.GetBytes(options.Serializer.Serialize(data));
                    return await PostContentAsync<T>(endpoint, encodedData, options, client, token).ConfigureAwait(false);
                }
            });
        }

      
        /// <summary>
        /// Posts content as HttpFormUrlEncodedContent, and returns parsed response data
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        public static Task<HttpResult<T>> PostFormEncodedAsync<T>(
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> formdata,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                // 3. Send and get a response
                return await PostContentAsync<T>(endpoint, GetFormUrlContentByteArray(formdata), options, client, token).ConfigureAwait(false);
            });
        }

        

        /// <summary>
        /// Posts content as HttpFormUrlEncodedContent. Ignores any response data from the server
        /// and checks only for HttpStatusCode == Success
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="formdata"></param>
        /// <param name="headers"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Task<HttpResult> PostFormEncodedNoResultAsync(
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> formdata,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            CancellationToken token = default)

        {
            return Task.Run(async () =>
            {
                // 2. Send and get a response
                return await PostWithNoResultAsync(endpoint, GetFormUrlContentByteArray(formdata), options, client, token).ConfigureAwait(false);
            });
        }
    }
}
