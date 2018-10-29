using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Universal.Framework.Serialization;
using Windows.Networking.Connectivity;

namespace Universal.Framework
{
    /// <summary>
    /// Legacy HTTP Client using System.Net.Http.
    /// Note: Does not implement returning all of the properties of
    /// HttpResult. For example, response headers are not returned.
    /// </summary>
    public static class HttpNET
    {
        #region Properties
        /// <summary>
        /// Default HTTP request timeout in seconds - 30 Seconds
        /// </summary>
        public const Int32 DEFAULT_TIMEOUT_SECONDS = 30;

        /// <summary>
        /// Default Cache expiry in Days - 5 minutes
        /// </summary>
        public static TimeSpan DEFAULT_CACHE_EXPIRY = TimeSpan.FromMinutes(5);

        private static Boolean _isConnectionAvailable = false;

        /// <summary>
        /// Returns whether or not we currently have an active internet connection
        /// </summary>
        public static Boolean IsConnectionAvailable
        {
            get { UpdateNetworkStatus(); return _isConnectionAvailable; }
        }

        /// <summary>
        /// Returns whether or not we should be restricting bandwidth. 
        /// This occurs if roaming, or if the GalleryData limit is being approached
        /// </summary>
        public static Boolean IsBandwidthRestricted
        {
            get;
            private set;
        }

        #endregion

        static Boolean Initialised = false;
        public static void Initialise()
        {
            if (!Initialised)
            {
                Initialised = true;
                NetworkInformation.NetworkStatusChanged += UpdateNetworkStatus;
                UpdateNetworkStatus();
            }
        }

        /// <summary>
        /// You typically shouldn't need to manually call this, but public just in case 
        /// race conditions are met with two separate classes hooking up to the 
        /// NetworkStatusChanged event and relying on IsConnectionAvailable
        /// </summary>
        /// <param name="sender"></param>
        public static void UpdateNetworkStatus(object sender = null)
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();

            if (profile == null)
            {
                _isConnectionAvailable = false;
                return;
            }


            if (profile.GetConnectionCost().ApproachingDataLimit
                || profile.GetConnectionCost().Roaming
                || profile.GetConnectionCost().OverDataLimit)
                IsBandwidthRestricted = true;

            var lvl = profile.GetNetworkConnectivityLevel();

            _isConnectionAvailable = (lvl == NetworkConnectivityLevel.InternetAccess);

        }

        /// <summary>
        /// Attempts to download and parse Serialized data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Uri"></param>
        /// <param name="serialiser">IMPORTANT - Only serializers that support String serialization will work</param>
        /// <param name="mode"></param>
        /// <param name="cacheExpiry"></param>
        /// <param name="cacheFolderName"></param>
        /// <param name="client"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<HttpResult<T>> GetDeserialisedAsync<T>(
            String Uri,
            ISerializer serialiser,
            CacheMode mode = CacheMode.UpdateImmediatelyIfExpired,
            TimeSpan? cacheExpiry = null,
            String cacheFolderName = Cache.HTTP_CACHE_FOLDERNAME,
            HttpClient client = null,
            Encoding encoding = null)
        {
            if (serialiser.SupportedModes() != SerializationMode.String)
                throw new NotSupportedException("Only string serializers are supported");

            // 1. Download the data
            var str = await GetStringAsync(Uri, mode, cacheExpiry, cacheFolderName, client, encoding).ConfigureAwait(false);

            // 2. Create the shell return object based on the downloaded data result
            var result = str.CreateBasedOn<T>();

            if (str.Success)
            {
                try
                {
                    // 3. Attempt to parse it
                    result.Result = await serialiser.DeserializeAsync<T>(str.Result).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    result.Success = false;
                    result.Exception = ex;
                }
            }

            return result;
        }


        /// <summary>
        /// Returns a string of HTTP data from the internet. If the data already exists in the cache, cached data is returned.
        /// If the cached data has expired, the cached data will be returned immediately, and then the cached data updated
        /// asynchronously.
        /// </summary>
        /// <param name="Uri"></param>
        /// <param name="cacheResult"></param>
        /// <param name="cacheExpiry"></param>
        /// <returns></returns>
        public static async Task<HttpResult<String>> GetStringAsync(
            String Uri,
            CacheMode mode = CacheMode.UpdateImmediatelyIfExpired,
            TimeSpan? cacheExpiry = null,
            String folderName = null,
            HttpClient client = null,
            Encoding encoding = null)
        {
            Initialise();
            HttpResult<String> result = new HttpResult<string>
            {
                OriginalUri = Uri,
                WasConnectionAvaliable = IsConnectionAvailable,
                Success = false,
                FromCache = false
            };

            // 2.  Attempt to retrieve cached data
            CacheResult<String> cachedata = null;
            if (mode != CacheMode.Skip)
            {
                cachedata = await Cache.GetStringFromCacheAsync(Uri, cacheExpiry, folderName);
                if (cachedata.Exists)
                {
                    result.Success = true;
                    result.FromCache = true;
                    result.Result = cachedata.Result;
                }
            }


            if (IsConnectionAvailable)
            {
                try
                {

                    bool didCache = false;
                    bool _isPrivateClient = false;
                    if (client == null)
                    {
                        _isPrivateClient = true;
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;
                        client = new HttpClient(handler);
                        client.Timeout = TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS);
                    }

                    // 3. If connection is available, and there is no cached data, 
                    //    or the cached data exists BUT the cache is set to update immediately,
                    //    or the cached data exists, is expired, and is set to update immediately if expired
                    //    for the data to be downloaded
                    if (String.IsNullOrEmpty(result.Result)
                        || mode == CacheMode.UpdateImmediately
                        || (mode == CacheMode.UpdateImmediatelyIfExpired && cachedata.Exists && cachedata.Expired))
                    {
                        if (encoding == null)
                        {
                            result.Result = await client.GetStringAsync(Uri);
                        }
                        else
                        {
                            var array = await client.GetByteArrayAsync(Uri);
                            result.Result = encoding.GetString(array, 0, array.Length);
                        }

                        result.FromCache = false;

                        if (_isPrivateClient)
                            client.Dispose();

                        result.Success = true;
                    }
                    else
                    {
                        // 4. Update the cache asynchronously if required
                        if (mode == CacheMode.UpdateAsync
                            || (cachedata.Expired && mode == CacheMode.UpdateAsyncIfExpired))
                        {
#pragma warning disable CS4014
                            Task.Run(async () =>
#pragma warning restore CS4014
{
                                var res = await GetStringAsync(Uri, CacheMode.Skip);
                                if (res.Success)
                                {
                                    didCache = true;
                                    //Cache.SaveToCacheAsync(Uri, res.Result, folderName);
                                }
                            });
                        }
                    }


                    if (!String.IsNullOrEmpty(result.Result) && mode != CacheMode.Skip)
                    {
                        try
                        {
                            //if (mode == CacheMode.UpdateImmediately || ((cachedata == null || cachedata.Expired || !cachedata.Exists) && mode == CacheMode.UpdateImmediatelyIfExpired))
                            //    await Cache.SaveToCacheAsync(Uri, result.Result, folderName);
                            //else if (mode == CacheMode.UpdateAsync || ((cachedata == null || cachedata.Expired || !cachedata.Exists) && mode == CacheMode.UpdateAsyncIfExpired))
                            //    Cache.SaveToCacheAsync(Uri, result.Result, folderName);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            // If we're told it's unauthorized access we don't want to fail the whole feed, the cache failed but the get didn't
                            Logger.Log(ex);
                        }
                    }



                }
                catch (TaskCanceledException tce)
                {
                    // We probably timed out here - BUT THIS SHOULD NOT BE REQUIRED?
                    if (cachedata != null && cachedata.Exists)
                    {
                        result.FromCache = true;
                        result.WasConnectionAvaliable = false;
                        result.Result = cachedata.Result;
                    }
                    else
                    {
                        result.Exception = tce;
                        Logger.Log(tce);
                    }
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                    result.Success = false;
                    Logger.Log(ex);
                }
            }

            // 5 - Return our final result!
            return result;
        }


    }
}
