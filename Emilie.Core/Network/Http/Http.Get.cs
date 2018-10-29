using System;
using System.Threading.Tasks;
using System.Threading;

namespace Emilie.Core.Network
{
    [QualityBand(QualityBand.Preview)]
    public static partial class Http
    {
        /* 
        /*
        /* This file contains all the methods related to HttpGet requests 
        /*
        */

        #region GET

        public static Task<HttpResult<T>> GetAsync<T>(
            Endpoint<T> endpoint,
            ICoreHttpClient client = null,
            Action<HttpResult<T>> updateCallback = null,
            CancellationToken token = default)
        {
            return GetAsync<T>(
                endpoint.Uri.AbsoluteUri,
                endpoint,
                client,
                updateCallback,
                token);
        }

        public static async Task<HttpResult<T>> GetAsync<T>(
            String Uri,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            Action<HttpResult<T>> updateCallback = null,
            CancellationToken token = default)
        {
            // 1. Create a callback wrapper around updateCallback that will pass back a correctly de-serialized result
            //    if we've requested an async update
            Action<HttpResult<byte[]>> callback = null;
            if (updateCallback != null)
            {
                callback = async (r) =>
                {
                    // 1.1. We need to parse the data before returning it, so....
                    HttpResult<T> result = await ParseAsync<T>(r, options.Serializer).ConfigureAwait(false);

                    // 1.2. Now we can invoke our top-level callback!
                    updateCallback(result);
                };
            }

            // 2. Download the data
            var byteResult = await GetRawAsync(Uri, options, client, callback, token).ConfigureAwait(false);

            // 3. Convert from HttpResult<byte[]> to HttpResult<T>
            return await ParseAsync<T>(byteResult, options.Serializer).ConfigureAwait(false);
        }

        public static async Task<HttpResult<Byte[]>> GetRawAsync(
            String Uri,
            HttpOptions options = null,
            ICoreHttpClient client = null,
            Action<HttpResult<byte[]>> updateCallback = null,
            CancellationToken token = default)
        {
            options = options ?? new HttpOptions();

            HttpResult<Byte[]> result = new HttpResult<Byte[]>
            {
                OriginalUri = Uri,
                WasConnectionAvaliable = IsConnectionAvailable,
                Success = false,
                FromCache = false
            };

            // 2.  Attempt to retrieve cached data
            INetworkCache cache = options.CacheOverride ?? Http.DefaultCache;
            CacheResult<Byte[]> cachedata = null;
            if (options.CacheMode != CacheMode.None && options.CacheMode != CacheMode.SystemManaged)
            {
                if (cache == null)
                    throw new ArgumentNullException("Http.DefaultCache", "The default HTTP cache has not been set. Using the Http cache will fail");

                string cacheKey = options.CacheKeyOverride ?? Uri;
                cachedata = await cache.GetBytesAsync(cacheKey, options.CacheExpiry).ConfigureAwait(false);
                if (cachedata.Exists)
                {
                    result.Success = true;
                    result.FromCache = true;
                    result.Content = cachedata.Result;
                }
            }

            if (IsConnectionAvailable)
            {
                try
                {
                    bool _isPrivateClient = false;
                    if (client == null)
                    {
                        _isPrivateClient = true;
                        client = (CacheMode.None == options.CacheMode) ? Http.NonCachingClient : Http.OptimisedClient;
                    }


                    // 3. If connection is available, and there is no cached data, 
                    //    or the cached data exists BUT the cache is set to update immediately,
                    //    or the cached data exists, is expired, and is set to update immediately if expired
                    //    for the data to be downloaded
                    if (result.Content == null
                        || options.CacheMode == CacheMode.UpdateImmediately
                        || (options.CacheMode == CacheMode.UpdateImmediatelyIfExpired && cachedata != null && cachedata.Exists && cachedata.Expired))
                    {
                        await SendInternalAsync(Uri, CoreHttpMethod.Get, options, client, result, token).ConfigureAwait(false);
                    }
                    else
                    {
                        // 4. Update the cache asynchronously if required
                        if (options.CacheMode == CacheMode.UpdateAsync || (cachedata.Expired && options.CacheMode == CacheMode.UpdateAsyncIfExpired))
                        {
                            _ = Task.Run(async () =>
                            {
                                // 4.1. Create our async result container
                                HttpResult<Byte[]> callbackResult = new HttpResult<Byte[]>
                                {
                                    OriginalUri = Uri,
                                    WasConnectionAvaliable = result.WasConnectionAvaliable,
                                    Success = false,
                                    FromCache = false
                                };

                                try
                                {
                                    // 4.2. Choose an appropriate Http Client for our request
                                    ICoreHttpClient cacheClient = (_isPrivateClient) ? Http.NonCachingClient : client;

                                    // 4.3. Attempt to download the data
                                    CoreHttpRequest cacheRequest = new CoreHttpRequest(Uri, CoreHttpMethod.Get, options);
                                    CoreHttpResponse res = await cacheClient.SendAsync(cacheRequest).ConfigureAwait(false);
                                    if (res.IsSuccessStatusCode)
                                    {
                                        // 4.4. Unbuffer the content
                                        callbackResult.Content = res.Content;
                                        callbackResult.Success = true;

                                        // 4.5. Save the new result to cache. 
                                        await cache.SaveAsync(Uri, res.Content).ConfigureAwait(false);
                                    }

                                    callbackResult.Exception = res.Exception;

                                    //// 4.6. If we made our own http client, dispose of it
                                    if (_isPrivateClient)
                                        cacheClient.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    // 4.7. Catch any exceptions we come across during the async update
                                    callbackResult.Exception = ex;
                                    Logger.Log(ex, "HttpCache AsyncUpdate");
                                }

                                // 4.8. If there's a callback, now is the time to let it know.
                                //      Keep in mind we're on a different thread now;
                                updateCallback?.Invoke(callbackResult);
                            });
                        }
                    }

                    if (result.Content != null
                        && options.CacheMode != CacheMode.None
                        && options.CacheMode != CacheMode.SystemManaged)
                    {
                        try
                        {
                            if (options.CacheMode == CacheMode.UpdateImmediately || ((cachedata == null || cachedata.Expired || !cachedata.Exists) && options.CacheMode == CacheMode.UpdateImmediatelyIfExpired))
                                await cache.SaveAsync(Uri, result.Content).ConfigureAwait(false);
                            else if (options.CacheMode == CacheMode.UpdateAsync || ((cachedata == null || cachedata.Expired || !cachedata.Exists) && options.CacheMode == CacheMode.UpdateAsyncIfExpired))
                            {
                                // Because this call is not awaited, execution of the current method continues before the call is completed
                                // We call this again even for UpdateAsync for *good* reason, that I'll go into detail at a later date... 
                                _ = cache.SaveAsync(Uri, result.Content).ConfigureAwait(false);
                            }
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            // If we're told it's unauthorized access we don't want to fail the whole feed, the cache failed but the get didn't
                            Logger.Log(ex);
                        }
                    }
                }
                catch (OperationCanceledException oce)
                {
                    // We probably timed out here - BUT THIS SHOULD NOT BE REQUIRED?
                    if (cachedata != null && cachedata.Exists)
                    {
                        result.FromCache = true;
                        result.WasConnectionAvaliable = false;
                        result.Content = cachedata.Result;
                    }
                    else
                    {
                        result.Exception = oce;
                        Logger.Log(oce);
                    }
                }
                catch (Exception ex)
                {
                    result.Exception = ex;

                    var details = GetConnectionExceptionDetails(ex);
                    if (!String.IsNullOrWhiteSpace(details) && cachedata != null && cachedata.Exists)
                    {
                        result.FromCache = true;
                        result.WasConnectionAvaliable = false;
                        result.Content = cachedata.Result;
                    }
                    else
                    {
                        result.Success = false;
                        Logger.Log(ex);
                    }
                }
            }

            // 5 - Return our final result!
            return result;
        }

        #endregion
    }
}
