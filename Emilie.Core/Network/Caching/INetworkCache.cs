using System;
using System.Threading.Tasks;

namespace Emilie.Core.Network
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
        /// Does not use either the system cache nor the framework cache. 
        /// </summary>
        None,
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
        UpdateImmediatelyIfExpired,
        /// <summary>
        /// Does not use Framework caching, but still uses a native system HTTP client that respects header caching if no client override is specified.
        /// </summary>
        SystemManaged
    }

    [QualityBand(QualityBand.PlannedRefactoring, "Considering removing String methods")]
    public interface INetworkCache
    {
        Int32 GetMaxEntries();

        Task<CacheResult<byte[]>> GetBytesAsync(String uri, TimeSpan? expiry);

        Task<bool> SaveAsync(string Uri, byte[] data);

        Task TrimAsync();

        Task InitialiseAsync();
    }
}
