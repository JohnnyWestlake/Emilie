using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Emilie.Core.Extensions;

namespace Emilie.UWP.Extensions
{
    /// <summary>
    /// Extension Methods for bytes, byte arrays and buffers
    /// </summary>
    public static class DataExtensions
    {
        public static string SafeDisplayName(this StorageFile file)
        {
            if (file.DisplayName.EndsWith(file.FileType))
                return file.DisplayName.Substring(0, file.DisplayName.LastIndexOf(file.FileType));

            return file.DisplayName;
        }


        /// <summary>
        /// Returns the byte array written too an InMemoryRandomAccessStream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Task<InMemoryRandomAccessStream> AsInMemoryRandomAccessStreamAsync(this byte[] bytes)
        {
            return bytes.AsBuffer().AsInMemoryRandomAccessStreamAsync();
        }

        public static async Task<InMemoryRandomAccessStream> AsInMemoryRandomAccessStreamAsync(this IBuffer buffer)
        {
            InMemoryRandomAccessStream mStream = new InMemoryRandomAccessStream();
            await mStream.WriteAsync(buffer);
            mStream.Seek(0);
            return mStream;
        }

        public static Task<byte[]> ToArrayAsync(this IRandomAccessStream stream)
        {
            return Task.Run(() =>
            {
                using (var s = stream.AsStream())
                {
                    return s.ToArray();
                }
            });
        }

        public static string AsString(this IRandomAccessStream stream)
        {
            using (var s = stream.AsStreamForRead())
            {
                return s.AsString();
            }
        }

        public static Task<string> AsStringAsync(this IRandomAccessStream stream)
            => Task.Run(() => AsString(stream));

        public static async Task<StorageFile> TryGetFileAsync(this StorageFolder folder, string fileName)
        {
            StorageFile file = null;
            file = await folder.TryGetItemAsync(fileName).ConfigureAwait(false) as StorageFile;
            return file;
        }
    }
}
