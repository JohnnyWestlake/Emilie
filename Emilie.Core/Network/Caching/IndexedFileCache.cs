using Emilie.Core;
using Emilie.Core.Storage;
using Emilie.Core.Utilities;
using SQLite;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Emilie.Core.Network
{
    public class IndexCacheEntry
    {
        [PrimaryKey]
        [Indexed]
        public string Key { get; set; }
        [Indexed]
        public DateTime DateAdded { get; set; }
        [Indexed]
        public DateTime DateLastAccessed { get; set; }
    }





    public class IndexedFileCache : INetworkCache
    {
        FileCache _fileCache = null;

        private SQLiteConnection _connection = null;
        private string _databasePath = null;

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public int MaxEntries = 300;

        ConcurrentDictionary<String, byte> _runtimeCache = null;

        string _fileName;
        string _folderName;

        public IndexedFileCache(String name)
        {
            _fileCache = new FileCache(name);

            _fileName = $"{name}.index";
            _folderName = name;

            _databasePath = Path.Combine(CoreIoC.Get<IStorageHelper>().GetAppCacheFolderPath(), _fileName);
            _connection = new SQLiteConnection(_databasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);

            _runtimeCache = new ConcurrentDictionary<string, byte>();
        }


        public Task<CacheResult<byte[]>> GetBytesAsync(string uri, TimeSpan? expiry)
        {
            return Task.Run(async () =>
            {
                var result = await _fileCache.GetBytesAsync(uri, expiry);
                if (result.Exists)
                {
                    _runtimeCache.TryAdd(uri, 0);
                    Task t = UpdateLastAccessedAsync(uri);
                }

                return result;
            });
        }

        Task UpdateLastAccessedAsync(string uri)
        {
            return Task.Run(() =>
            {
                try
                {
                    _semaphore.Wait();

                    if (!DoesCacheTableExist(_connection))
                        return;

                    _connection.RunInTransaction(() =>
                    {
                        IndexCacheEntry entry = _connection.Find<IndexCacheEntry>(e => e.Key == uri);
                        if (entry != null)
                        {
                            entry.DateLastAccessed = DateTime.Now;
                            _connection.Update(entry);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        public int GetMaxEntries()
        {
            return 500;
        }

        public Task InitialiseAsync()
        {
            return Task.Run(async () =>
            {
                await _fileCache.InitialiseAsync().ConfigureAwait(false);
                if (!DoesCacheTableExist(_connection))
                {
                    _connection.CreateTable<IndexCacheEntry>();
                }
            });
        }

        public Task<bool> SaveAsync(string Uri, byte[] data)
        {
            return Task.Run(async () =>
            {
                bool saved = await _fileCache.SaveAsync(Uri, data).ConfigureAwait(false);

                if (saved)
                {
                    try
                    {
                        _semaphore.Wait();

                        if (!DoesCacheTableExist(_connection))
                            return false;

                        _connection.RunInTransaction(() =>
                        {
                            IndexCacheEntry entry = _connection.Find<IndexCacheEntry>(e => e.Key == Uri);
                            if (entry == null)
                            {
                                DateTime time = DateTime.Now;
                                _connection.Insert(new IndexCacheEntry
                                {
                                    Key = Uri,
                                    DateAdded = time,
                                    DateLastAccessed = time
                                });
                            }
                        });

                        _runtimeCache.TryAdd(Uri, 0);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        saved = false;
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }

                return saved;
            });
        }

        /// <summary>
        /// Returns the local file system path to a cached piece of content.
        /// Cache must be initialized before called this method.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPath(string uri)
        {
            if (_fileCache.CacheFolder == null)
                throw new InvalidOperationException("Cache has not yet been initialized yet");

            return String.Format("{0}\\{1}", _fileCache.CacheFolder.Path, Cryptography.GetMD5Hash(uri));
        }

        public string GetApplicationDataPath(string uri)
        {
            if (_fileCache.CacheFolder == null)
                throw new InvalidOperationException("Cache has not yet been initialized yet");

            return $"ms-appdata:///localcache/{_folderName}/{Cryptography.GetMD5Hash(uri)}";
        }

        public Task SaveAsync(string Uri, string result)
        {
            throw new NotImplementedException();
        }

        public Task TrimAsync()
        {
            throw new NotImplementedException();

            // todo : arranged items by DateLastAccessed, then delete the oldest ones
        }

        bool DoesCacheTableExist(SQLiteConnection c) => c.GetTableInfo(nameof(IndexCacheEntry)).Count > 0;


        public Task<bool> ContainsAsync(string uri)
        {
            if (_runtimeCache.TryGetValue(uri, out byte result))
                return Task.FromResult(true);

            return Task.Run(() =>
            {
                try
                {
                    //_semaphore.Wait();

                    if (!DoesCacheTableExist(_connection))
                        return false;

                    IndexCacheEntry entry = _connection.Find<IndexCacheEntry>(e => e.Key == uri);
                    if (entry != null)
                        _runtimeCache.TryAdd(uri, 0);

                    return entry != null;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                finally
                {
                    //_semaphore.Release();
                }

                return false;
            });
        }

        public bool Contains(string uri)
        {
            if (_runtimeCache.TryGetValue(uri, out byte result))
                return true;

            try
            {
                //_semaphore.Wait();

                //if (!DoesCacheTableExist(_connection))
                //    return false;

                IndexCacheEntry entry = _connection.Find<IndexCacheEntry>(e => e.Key == uri);
                if (entry != null)
                    _runtimeCache.TryAdd(uri, 0);

                return entry != null;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            //finally
            //{
            //    _semaphore.Release();
            //}

            return false;
        }

    }
}
