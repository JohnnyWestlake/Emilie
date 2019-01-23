using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Emilie.Core.Common;
using Emilie.Core.Compression;
using Emilie.Core.Extensions;
using Emilie.Core.Storage;

namespace Emilie.Core.Network
{
    public class SQLiteCache : BasicDefaultable<SQLiteCache>, INetworkCache, IDisposable
    {
        #region Cache Instances

        private static SQLiteCache _compressedCache = null;

        /// <summary>
        /// Returns a GZIP compressed network cache
        /// </summary>
        public static SQLiteCache CompressedInstance => _compressedCache ?? (_compressedCache = new SQLiteCache(@"\netcache.cdb", true, GZip.Default));

        #endregion

        private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private string _databasePath { get; }
        private IStreamCompressor _compressor { get; }
        public SQLiteConnection Connection { get; }

        public int MaxEntries = 300;




        public SQLiteCache() : this("networkcache.udb") { }

        public SQLiteCache(String databasepath, bool isPathRelative = true, IStreamCompressor compressor = null)
        {
            _databasePath = isPathRelative 
                ? CoreIoC.Get<IStorageHelper>().GetAppCacheFolderPath() + @"\" + databasepath 
                : databasepath;

            Connection = new SQLiteConnection(_databasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            Connection.CreateTable<SQLCacheEntry>();
            _compressor = compressor;
        }

        public SQLiteCache SetLockModeExclusive()
        {
            Connection.ExecuteScalar<string>("PRAGMA main.locking_mode=EXCLUSIVE");
            return this;
        }

        public SQLiteCache SetLockModeNormal()
        {
            Connection.ExecuteScalar<string>("PRAGMA main.locking_mode=NORMAL");
            return this;
        }

        public int GetMaxEntries() => MaxEntries;

        private static ulong CreateHash64(string str)
        {
            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(str);

            ulong value = (ulong)utf8.Length;
            for (int n = 0; n < utf8.Length; n++)
            {
                value += (ulong)utf8[n] << ((n * 5) % 56);
            }

            return value;
        }

        public Task<CacheResult<byte[]>> GetBytesAsync(string uri, TimeSpan? expiry = null)
        {
            return Task.Run(async () =>
            {
                CacheResult<byte[]> result = new CacheResult<byte[]>();
                string hashed = CreateHash64(uri).ToString();
                try
                {
                    _rwLock.EnterReadLock();

                    // If table doesn't exist, there's no data to get.
                    if (!DoesCacheTableExist(Connection))
                    {
                        result.Exists = false;
                    }
                    else
                    {
                        // Check to see if there's a matching entry in the DB
                        SQLCacheEntry entry = Connection.Find<SQLCacheEntry>(e => e.Key == hashed);
                        if (entry == null)
                        {
                            result.Exists = false;
                        }
                        else
                        {
                            // If there is, get the data and set the expiry times.
                            // For this cache, we currently don't have a "last accessed" property on cache entries
                            result.Result = entry.Data;
                            result.Exists = true;
                            if (expiry == null || !expiry.HasValue)
                            {
                                result.Expired = false;
                            }
                            else if (expiry.HasValue)
                            {
                                result.Expired = (DateTime.Now - entry.DateAdded > expiry.Value);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }

                try
                {
                    // We attempt decompression *outside* of the ReadLock, just so we're not wasting the
                    // time of anything wanting to access the WriteLock
                    if (result != null && result.Exists && result.Result != null && _compressor != null)
                    {
                        result.Result = await DecompressDataAsync(result.Result).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                return result;
            });
        }

        public Task<bool> SaveAsync(string uri, byte[] data)
        {
            return Task.Run(async () =>
            {
                string hashed = CreateHash64(uri).ToString();
                byte[] blob = (_compressor == null) ? data : await CompressDataAsync(data).ConfigureAwait(false);
                data = null;
                bool result = false;

                try
                {
                    _rwLock.EnterWriteLock();
                    Connection.RunInTransaction(() =>
                    {
                        if (!DoesCacheTableExist(Connection))
                            Connection.CreateTable<SQLCacheEntry>();

                        SQLCacheEntry entry = Connection.Find<SQLCacheEntry>(e => e.Key == hashed);

                        if (entry != null)
                        {
                            entry.Data = blob;
                            entry.DateLastAccessed = DateTime.Now;
                            Connection.Update(entry);
                        }
                        else
                        {
                            entry = new SQLCacheEntry()
                            {
                                DateLastAccessed = DateTime.Now,
                                DateAdded = DateTime.Now,
                                Data = blob,
                                Key = hashed
                            };

                            Connection.Insert(entry);
                        }

                        result = true;
                    });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }

                return result;
            });
        }

        public Task InitialiseAsync() => Task.FromResult(0);

        public Task TrimAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    int itemCount = 0;

                    Connection.RunInTransaction(() =>
                    {
                        TableQuery<SQLCacheEntry> toodelete;
                        try
                        {
                            _rwLock.EnterReadLock();

                            // If no cache table, nothing to trim!
                            if (!DoesCacheTableExist(Connection))
                                return;

                            // Count the items - we only care if we're over the count in this case
                            itemCount = Connection.Table<SQLCacheEntry>().Count();
                            if (itemCount <= GetMaxEntries())
                                return;
                        }
                        finally
                        {
                            _rwLock.ExitReadLock();
                        }


                        try
                        {

                            _rwLock.EnterWriteLock();

                            // Find what we want to delete
                            toodelete = Connection.Table<SQLCacheEntry>().OrderBy(entry => entry.DateLastAccessed).Take(itemCount - GetMaxEntries());

                            // Delete all the items we need too
                            foreach (var entry in toodelete.ToList())
                                Connection.Delete(entry);
                        }
                        finally
                        {
                            _rwLock.ExitWriteLock();
                        }


                    });

                    if (itemCount < GetMaxEntries())
                    {
                        try
                        {
                            // Clear the blank space
                            _rwLock.EnterWriteLock();
                            Vacuum();
                        }
                        finally
                        {
                            _rwLock.ExitWriteLock();
                        }
                    }

                }
                catch { }

            });
        }



        public Task ClearCacheAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    _rwLock.EnterWriteLock();

                    Connection.RunInTransaction(() =>
                    {
                        if (DoesCacheTableExist(Connection))
                            Connection.DropTable<SQLCacheEntry>();

                        Connection.CreateTable<SQLCacheEntry>();
                    });

                    Vacuum();
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            });
        }

        /// <summary>
        /// WARNING: This cannot be done inside a transaction.
        /// Compress' database by removing blankspace / holes / fragmentation inside the data structure
        /// </summary>
        void Vacuum()
        {
            SQLiteCommand vacuum = Connection.CreateCommand("vacuum");
            vacuum.ExecuteNonQuery();
        }

        bool DoesCacheTableExist(SQLiteConnection c)
        {
            if (c.TableMappings.Any())
                return true;

            return false;
        }


        #region Compression Helpers


        private async Task<byte[]> CompressDataAsync(byte[] data)
        {
            byte[] bytes = null;

            using (MemoryStream destination = new MemoryStream())
            using (var stream = data.AsStream())
            {
                bytes = await _compressor.CompressToBytesAsync(stream).ConfigureAwait(false);
            }

            return bytes;
        }

        private async Task<byte[]> DecompressDataAsync(byte[] data)
        {
            byte[] bytes = null;

            using (var dataStream = data.AsStream())
            using (var resultStream = new MemoryStream())
            {
                // 3. Decompress the data from the file stream into our buffer stream
                await _compressor.DecompressToAsync(resultStream, dataStream).ConfigureAwait(false);

                // 5. Return our buffer as a basic stream
                bytes = resultStream.ToArray();
            }

            return bytes;
        }



        #endregion



        public void Dispose()
        {
            Connection.Close();
        }
    }
}
