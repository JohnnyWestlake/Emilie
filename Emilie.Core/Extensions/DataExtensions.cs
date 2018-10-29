using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Emilie.Core.Extensions
{
    /// <summary>
    /// Extension Methods for bytes, byte arrays and buffers
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Converts a byte array into a string. If no encoder is specified, UTF8 encoder is used by default.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string AsString(this byte[] bytes, Encoding encoder = null)
        {
            if (encoder == null)
                encoder = Encoding.UTF8;

            return encoder.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] ToBytes(this string text) => Encoding.UTF8.GetBytes(text);

        /// <summary>
        /// Returns a task that returns the data this byte array represents as a stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Task<MemoryStream> AsStreamAsync(this byte[] bytes)
        {
            return Task.Run(() => { return AsStream(bytes); });
        }

        public static MemoryStream AsStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }


        public static Task<byte[]> ToArrayAsync(this Stream stream)
        {
            return Task.Run(() => stream.ToArray());
        }

        public static byte[] ToArray(this Stream stream)
        {
            // Performance optimisation for MemoryStream
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray();

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            return bytes;
        }

        public static String AsString(this Stream stream, Encoding encoder = null)
        {
            String result = null;
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            };

            return result;
        }
    }
}
