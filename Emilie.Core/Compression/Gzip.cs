using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Emilie.Core.Common;

namespace Emilie.Core.Compression
{
    /// <summary>
    /// Provides ability to compress and decompress stream data using the Gzip compression format
    /// </summary>
    public class GZip : BasicDefaultable<GZip>, IStreamCompressor
    {
        // Only one async read/write operation allowed by the framework apparently ;___;
        //readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public void CompressTo(Stream data, Stream destination, CompressionLevel level = CompressionLevel.Optimal)
        {
            using (var compressionStream = new GZipStream(destination, level, true))
            {
                data.CopyTo(compressionStream);
            }
        }

        public Task CompressToAsync(Stream data, Stream destination, CompressionLevel level = CompressionLevel.Optimal)
        {
            return Task.Run(() => CompressTo(data, destination, level));
        }

        public Task<byte[]> CompressToBytesAsync(Stream data, CompressionLevel level = CompressionLevel.Optimal)
        {
            return Task.Run(() =>
            {
                //await semaphore.WaitAsync().ConfigureAwait(false);
                byte[] bytes = null;
                try
                {
                    using (MemoryStream destination = new MemoryStream())
                    {
                        using (var compressionStream = new GZipStream(destination, level))
                        {
                            data.CopyTo(compressionStream);
                        }

                        bytes = destination.ToArray();
                    }
                }
                finally
                {
                    //semaphore.Release();
                }

                return bytes;
            });
        }

        public void DecompressTo(Stream input, Stream decompressedOutputTarget)
        {
            using (var compressionStream = new GZipStream(input, CompressionMode.Decompress, true))
            {
                compressionStream.CopyTo(decompressedOutputTarget);
            }
        }
        public Task DecompressToAsync(Stream destination, Stream source)
        {
            return Task.Run(() => DecompressTo(source, destination));
        }
    }
}