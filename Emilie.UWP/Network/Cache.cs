using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.Framework;
using Universal.Framework.Utilities;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Universal.Framework.Extensions;
using Universal.Framework.Compression;

namespace Universal.Framework
{
    /// <summary>
    /// A simple wrapper class for returning
    /// generic cached data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheResult<T>
    {
        public T Result { get; set; }
        public Boolean Exists { get; set; }
        public Boolean Expired { get; set; }
    }

    /// <summary>
    /// Dictates how the caching mechanism should operate
    /// </summary>
    public enum CacheMode
    {
        /// <summary>
        /// Do not use the cache
        /// </summary>
        Skip,
        /// <summary>
        /// Returns the cached item immediately, then updates the cache asynchronously regardless of expiry time. If offline, returns cached data.
        /// </summary>
        UpdateAsync,
        /// <summary>
        /// Returns the cached item immediately. If expired, then updates the cache asynchronously. If offline, returns cached data.
        /// </summary>
        UpdateAsyncIfExpired,
        /// <summary>
        /// Updates the cache first and then returns the result regardless of expiry time. If offline, returns cached data.
        /// </summary>
        UpdateImmediately,
        /// <summary>
        /// If expired, updates the cache first and then returns the result. Otherwise returns the cached item immediately. If offline, returns cached data.
        /// </summary>
        UpdateImmediatelyIfExpired
    }

    public static class Cache
    {
        /// <summary>
        /// Defines the default folder name for the HTTP String Cache. Default: "HttpStringCache"
        /// </summary>
        public const String HTTP_CACHE_FOLDERNAME = "HttpCache";
        /// <summary>
        /// Set this to define the default caching location
        /// </summary>
        public static StorageFolder HttpCacheFolder = null;

        /// <summary>
        /// The maximum amount of files allowed in HttpStringCache - 300 files
        /// </summary>
        public static Int32 MAX_CACHE_FILE_COUNT = 150;

        /// <summary>
        /// 40 MB Cap on HTTP Cache
        /// NOT IMPLEMENTED YET.
        /// </summary>
        public static Int32 MAX_HTTP_CACHE_FILE_SIZE_MB = 40;

        /// <summary>
        /// This cache is never automatically trimmed. Be careful what you put in here,
        /// make sure you're using suitable expiry times if need be
        /// </summary>
        public const string PERMA_CACHE_FOLDER_NAME = "PermaCache";


        public static async Task InitialiseCacheAsync()
        {
            if (HttpCacheFolder == null)
            {
                var root = Windows.Storage.ApplicationData.Current.LocalFolder;
                HttpCacheFolder = await root.CreateFolderAsync(HTTP_CACHE_FOLDERNAME, CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Attempts to get a cache result for a given URI endpoint
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="expiry"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static async Task<CacheResult<String>> GetStringFromCacheAsync(String uri, TimeSpan? expiry = null, String folderName = HTTP_CACHE_FOLDERNAME)
        {
            var byteResult = await GetDataFromCacheAsync(uri, expiry, folderName).ConfigureAwait(false);

            CacheResult<String> result = new CacheResult<String>
            {
                Exists = byteResult.Exists,
                Expired = byteResult.Expired,
                Result = null
            };

            if (byteResult.Result != null)
            {
                result.Result = await byteResult.Result.AsStringAsync().ConfigureAwait(false);
            }

            return result;
        }

        public static async Task<CacheResult<byte[]>> GetDataFromCacheAsync(String uri, TimeSpan? expiry = null, String folderName = HTTP_CACHE_FOLDERNAME)
        {
            await InitialiseCacheAsync().ConfigureAwait(false);

            CacheResult<Byte[]> result = new CacheResult<Byte[]>
            {
                Exists = false,
                Expired = false,
                Result = null
            };

            StorageFile file = null;
            StorageFolder folder = await GetTargetFolderAsync(folderName).ConfigureAwait(false);


            var fileName = Cryptography.GetMD5Hash(uri);
            try
            {
                // Check to see if file exists in cache folder.
                // Exception is thrown if not
                file = await folder.GetFileAsync(fileName);

                if (file != null)
                {
                    // If we care about the expiry data, work out
                    // if the data has expired
                    if (expiry != null && expiry.HasValue)
                    {
                        var date = file.DateCreated;
                        var diff = DateTime.Now - file.DateCreated;
                        result.Expired = (diff.TotalMinutes > expiry.Value.Minutes);
                    }

                    // Obtain the decompressed byte array from the file
                    result.Result = await Files.ReadCompressedBytesAsync(file, GZip.Instance).ConfigureAwait(false);
                    result.Exists = true;

                    // Update last accessed date by overwriting the file
                    if (!result.Expired)
                    {
                        var str = result.Result;
#pragma warning disable CS4014
                        SaveToCacheAsync(uri, str, folder);
#pragma warning restore CS4014
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // Doesn't exist in cache, continue with life.
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            return result;
        }

        /// <summary>
        /// Attempts to get the cache folder inside local storage. If the folder cannot be found,
        /// the folder is created.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private static async Task<StorageFolder> GetTargetFolderAsync(string folderName)
        {
            StorageFolder folder = null;

            if (String.IsNullOrEmpty(folderName) || folderName.Equals(HTTP_CACHE_FOLDERNAME))
            {
                folder = HttpCacheFolder;
            }
            else
            {
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            }

            return folder;
        }

        /// <summary>
        /// If the input folder is null, the default HttpCacheFolder is returned
        /// </summary>
        /// <param name="_folder"></param>
        /// <returns></returns>
        private static async Task<StorageFolder> GetTargetFolderAsync(StorageFolder _folder)
        {
            StorageFolder folder = _folder;

            if (_folder == null)
                folder = HttpCacheFolder;

            return _folder;
        }


        /// <summary>
        /// Saves the result of a string HTTP request to a file
        /// </summary>
        /// <param name="Uri">Uri of the original request. This will be used to generate a unique file hash to use as the cache key</param>
        /// <param name="result">The data to be cached</param>
        /// <param name="_folder">The folder to cache the data in. If left null, the default HTTP Cache folder is used</param>
        /// <returns></returns>
        public static Task SaveHttpStringToCacheAsync(string Uri, string result, StorageFolder _folder = null)
        {
            return SaveToCacheAsync(Uri, Encoding.UTF8.GetBytes(result), _folder);
        }

        /// <summary>
        /// Saves the result of a string HTTP request to a file
        /// </summary>
        /// <param name="Uri">Uri of the original request. This will be used to generate a unique file hash to use as the cache key</param>
        /// <param name="result">The data to be cached</param>
        /// <param name="folderName">The name of the folder in local storage to cache the data in. If the folder cannot be found, it is created.</param>
        /// <returns></returns>
        public static Task SaveHttpStringToCacheAsync(string Uri, string result, string folderName = null)
        {
            return SaveToCacheAsync(Uri, Encoding.UTF8.GetBytes(result), folderName);
        }

        public static async Task SaveToCacheAsync(string Uri, byte[] data, string _folderName = null)
        {
            StorageFolder _folder = await GetTargetFolderAsync(_folderName).ConfigureAwait(false);
            await SaveToCacheAsync(Uri, data, _folder).ConfigureAwait(false);
        }

        public static async Task SaveToCacheAsync(string Uri, byte[] data, StorageFolder _folder = null)
        {
            try
            {
                // 1. Attempt to open the correct folder
                StorageFolder folder = await GetTargetFolderAsync(_folder).ConfigureAwait(false);

                // 2. Open / Create the file
                StorageFile file = await folder.CreateFileAsync(Cryptography.GetMD5Hash(Uri), CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);

                // 2.1. A workaround to properly update the cache date, as ReplaceExisting does not
                //      overwrite the DateCreated meta-data on the file (for unknown reasons)
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete).ConfigureAwait(false);
                file = await folder.CreateFileAsync(Cryptography.GetMD5Hash(Uri), CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);

                try
                {
                    // 3. Write the cached result
                    //    (We use a transaction here to attempt to prevent cache corruption);
                    using (var transaction = await file.OpenTransactedWriteAsync().AsTask().ConfigureAwait(false))
                    using (var dataStream = await data.AsStreamAsync().ConfigureAwait(false))
                    {
                        transaction.Stream.Seek(0);
                        await GZip.Instance.CompressToAsync(dataStream, transaction.Stream.AsStream()).ConfigureAwait(false);
                        await transaction.CommitAsync().AsTask().ConfigureAwait(false);
                    }
                }
                catch (Exception)
                {
                    //Logger.Log(ex);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }




        #region Cache Maintenance & Management


        /// <summary>
        /// Keeps the HttpStringCache to a manageable size
        /// </summary>
        /// <returns></returns>
        public static async Task TrimHttpStringCacheAsync()
        {
            await InitialiseCacheAsync().ConfigureAwait(false);
            var files = await HttpCacheFolder.GetFilesAsync().AsTask().ConfigureAwait(false);

            if (files.Count > MAX_CACHE_FILE_COUNT)
            {
                await Task.Run(async () =>
                {
                    var ordered = files.OrderBy(f => f.DateCreated).Take(50).ToList();
                    foreach (var file in ordered)
                    {
                        try
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
