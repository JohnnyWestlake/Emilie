using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Emilie.Core.Compression
{
    public interface IStreamCompressor
    {
        void CompressTo(Stream source, Stream destination, CompressionLevel level);
        Task CompressToAsync(Stream source, Stream destination, CompressionLevel level);
        Task<byte[]> CompressToBytesAsync(Stream data, CompressionLevel level = CompressionLevel.Optimal);
        void DecompressTo(Stream source, Stream decompressedOutputTarget);
        Task DecompressToAsync(Stream destination, Stream source);
    }
}
