using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Emilie.Core.Compression;
using Emilie.Core.Extensions;
using Emilie.Core.Storage;
using Emilie.Core.Utilities;

namespace Emilie.Core.Network
{
    /// <summary>
    /// Caches data as flat files in LocalStorage, with optional compression.
    /// </summary>
    [QualityBand(QualityBand.Preview, "Requires testing around Trim Method")]
    public class FileCache : INetworkCache
    {
        //------------------------------------------------------
        //
        //  Constants
        //
        //------------------------------------------------------

        private const int DEFAULT_MAX_FILES = 100;

        private const String DEFAULT_FOLDERNAME = "HttpCache";




        //------------------------------------------------------
        //
        //  Properties
        //
        //------------------------------------------------------

        private IStreamCompressor _compressor = null;

        private IStorageHelper _storageHelper { get; } = CoreIoC.Get<IStorageHelper>();

        protected String FolderName { get; }

        public Int32 MaxFileCount { get; }

        public IFolder HttpCacheFolder { get; private set; }




        //------------------------------------------------------
        //
        //  Constructor
        //
        //------------------------------------------------------

        public FileCache()
            : this(DEFAULT_FOLDERNAME, DEFAULT_MAX_FILES) { }

        public FileCache(IStreamCompressor compressor, int maxFileCount = DEFAULT_MAX_FILES)
            : this(DEFAULT_FOLDERNAME, compressor, maxFileCount) { }

        public FileCache(String folderName, int maxFileCount = DEFAULT_MAX_FILES)
            : this(folderName, null, maxFileCount) { }

        public FileCache(
            String folderName, IStreamCompressor compressor, int maxFileCount = DEFAULT_MAX_FILES)
        {
            _compressor = compressor;
            FolderName = folderName;
            MaxFileCount = maxFileCount;
        }

        public FileCache
            (IFolder cacheFolder, IStreamCompressor compressor = null, int maxFileCount = DEFAULT_MAX_FILES)
        {
            _compressor = compressor;
            HttpCacheFolder = cacheFolder;
            FolderName = cacheFolder.Name;
        }




        //------------------------------------------------------
        //
        //  Implementation
        //
        //------------------------------------------------------

        public Task InitialiseAsync()
        {
            if (HttpCacheFolder != null)
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                IFolder root = await _storageHelper.GetFolderFromPathAsync(_storageHelper.GetAppCacheFolderPath()).ConfigureAwait(false);
                HttpCacheFolder = await root.CreateOrOpenFolderAsync(FolderName).ConfigureAwait(false);
            });
        }

        string GetKey(string key)
        {
            return Cryptography.GetDJB2Hash(key).ToString();
        }

        /// <summary>
        /// Returns the local file system path to a cached piece of content.
        /// Cache must be initialized before called this method.
        /// </summary>
        /// <exception cref="InvalidOperationException">If HttpCache has not yet been initialized.</exception>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPath(string uri)
        {
            if (HttpCacheFolder == null)
                throw new InvalidOperationException("Cache has not yet been initialized yet");

            return Path.Combine(HttpCacheFolder.Path, Cryptography.GetMD5Hash(uri));
        }

        public Task<CacheResult<byte[]>> GetBytesAsync(string uri, TimeSpan? expiry)
        {
            return Task.Run(async () =>
            {
                await InitialiseAsync().ConfigureAwait(false);

                CacheResult<Byte[]> result = new CacheResult<Byte[]>
                {
                    Exists = false,
                    Expired = false,
                    Result = null
                };

                IFile file = null;
                IFolder folder = HttpCacheFolder;

                String fileName = GetKey(uri);

                try
                {
                    // Check to see if file exists in cache folder.
                    file = await folder.TryGetFileAsync(fileName).ConfigureAwait(false);

                    if (file != null)
                    {
                        // If we care about the expiry data, work out
                        // if the data has expired
                        if (expiry != null && expiry.HasValue)
                        {
                            var date = file.DateCreatedUtc;
                            var diff = DateTime.UtcNow - file.DateCreatedUtc;
                            result.Expired = (diff.Ticks > expiry.Value.Ticks);
                        }

                        // Obtain the decompressed byte array from the file
                        if (_compressor != null)
                        {
                            result.Result = await Files.ReadCompressedBytesAsync(file, _compressor).ConfigureAwait(false);
                        }
                        else
                        {
                            using (Stream stream = await file.OpenReadAsync().ConfigureAwait(false))
                            {
                                result.Result = stream.ToArray();
                            }
                        }

                        result.Exists = true;

                        // Update last accessed date by overwriting the file
                        if (!result.Expired)
                        {
                            _ = UpdateLastModifiedAsync(file);
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
                    result.Exists = false;
                }

                return result;
            });
        }

        public int GetMaxEntries() => MaxFileCount;

        private async Task UpdateLastModifiedAsync(IFile file)
        {
            try
            {
                await file.UpdateLastModifiedAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public Task SaveAsync(string Uri, byte[] data)
        {
            return Task.Run(async () =>
            {
                try
                {
                    // 1. Attempt to open the correct folder and target file name
                    IFolder folder = HttpCacheFolder;
                    String hash = GetKey(Uri);

                    // 2. Delete the old file if existing
                    //    This is required as overwriting the file does *not* update the DateModified
                    //    property, which we use to clean the cache
                    IFile file = await folder.TryGetFileAsync(hash).ConfigureAwait(false);
                    if (file != null)
                        await file.DeleteAsync().ConfigureAwait(false);

                    // 2.1. Now create the file for writing
                    file = await folder.CreateOrOpenFileAsync(hash).ConfigureAwait(false);

                    // 3. Write the result. Unfortunately transacted streams cause many number of problems,
                    //    so we avoid using them here.
                    using (Stream stream = await file.OpenWriteAsync().ConfigureAwait(false))
                    {
                        if (_compressor != null)
                        {
                            using (Stream dataStream = data.AsStream())
                            {
                                await _compressor.CompressToAsync(dataStream, stream, System.IO.Compression.CompressionLevel.Optimal).ConfigureAwait(false);
                            }
                        }
                        else
                            stream.Write(data, 0, data.Length);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }



        public Task TrimAsync()
        {
            return Task.Run(async () =>
            {
                await InitialiseAsync().ConfigureAwait(false);
                var files = await HttpCacheFolder.GetFilesAsync().ConfigureAwait(false);


                // TODO : Can implement by interfacing out GetFilesByDateAsync(SortOrder),
                //        then UWP side can use file queries
                if (files.Count > MaxFileCount)
                {
                    var ordered = files.OrderBy(f => f.DateCreatedUtc).Take(MaxFileCount / 2).ToList();
                    foreach (var file in ordered)
                    {
                        try
                        {
                            await file.DeleteAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            });
        }
    }
}
