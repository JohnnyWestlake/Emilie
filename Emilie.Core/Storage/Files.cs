using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Emilie.Core.Compression;
using Emilie.Core.Extensions;
using Emilie.Core.Serialization;

namespace Emilie.Core.Storage
{
    public static class Files
    {
        public static string GetSafeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
            {
                name = name.Replace(c, ' ');
            }

            return name;
        }

        public static Task<byte[]> GetBytesAsync(IFile file)
        {
            return Task.Run(async () =>
            {
                byte[] bytes;

                using (Stream stream = await file.OpenReadAsync().ConfigureAwait(false))
                {
                    bytes = stream.ToArray();
                }

                return bytes;
            });
        }

        public static Task<string> ReadTextAsync(IFile file, Encoding encoding = null)
        {
            return Task.Run(async () =>
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;

                using (Stream stream = await file.OpenReadAsync().ConfigureAwait(false))
                {
                    return encoding.GetString(stream.ToArray());
                }
            });
        }

        public static Task WriteTextAsync(IFile file, string text, Encoding encoding = null)
        {
            return Task.Run(async () =>
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;

                using (Stream stream = await file.OpenWriteAsync().ConfigureAwait(false))
                {
                    byte[] bytes = encoding.GetBytes(text);
                    stream.Write(bytes, 0, bytes.Length);
                }
            });
        }





        #region ReadSerialized

        /// <summary>
        /// Reads an object from a file using a serializer to deserialize
        /// the file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">File to read the object from</param>
        /// <param name="serializer">Serializer used to deserialize the object</param>
        /// <param name="compressor">Optional compressor to decompress the written file</param>
        /// <returns>Deserialized Object</returns>
        public static Task<T> ReadObjectAsync<T>(
            IFile file,
            ISerializer serializer,
            IStreamCompressor compressor = null)
        {
            if (serializer.SupportedModes().HasFlag(SerializationMode.Stream))
                return ReadSerializedStreamAsync<T>(file, serializer, compressor);

            return ReadSerializedStringAsync<T>(file, serializer, compressor);
        }

        private static Task<T> ReadSerializedStringAsync<T>(
            IFile file,
            ISerializer serializer,
            IStreamCompressor compressor = null)
        {
            return Task.Run(async () =>
            {
                T result = default;

                if ((serializer.SupportedModes() & SerializationMode.String) != 0)
                    throw new NotSupportedException("The given serializer does not support string deserializing");

                try
                {
                    String text;
                    if (compressor != null)
                    {
                        using (var dataStream = new MemoryStream())
                        using (var fileStream = await file.OpenReadAsync().ConfigureAwait(false))
                        {
                            await compressor.DecompressToAsync(fileStream, dataStream).ConfigureAwait(false);
                            dataStream.Seek(0, SeekOrigin.Begin);
                            text = dataStream.AsString();
                        }
                    }
                    else
                    {
                        text = await file.ReadTextAsync().ConfigureAwait(false);
                    }

                    result = serializer.Deserialize<T>(text);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return result;
                }

                return result;
            });
        }

        private static Task<T> ReadSerializedStreamAsync<T>(
            IFile file,
            ISerializer serializer,
            IStreamCompressor compressor = null)
        {
            return Task.Run(async () =>
            {
                T result = default(T);

                if (!(serializer.SupportedModes().HasFlag(SerializationMode.Stream)))
                    throw new NotSupportedException("The given serializer does not support stream deserializing");

                try
                {
                    using (var fileStream = await file.OpenReadAsync().ConfigureAwait(false))
                    {
                        if (compressor == null)
                        {
                            result = serializer.Deserialize<T>(fileStream);
                        }
                        else
                        {
                            using (var dataStream = new MemoryStream())
                            {
                                await compressor.DecompressToAsync(dataStream, fileStream).ConfigureAwait(false);
                                dataStream.Seek(0, SeekOrigin.Begin);
                                result = serializer.Deserialize<T>(dataStream);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return result;
                }

                return result;
            });
        }

        #endregion

        #region WriteSerialized

        /// <summary>
        /// Writes an object to a File using the given serializer.
        /// </summary>
        /// <typeparam name="T">Type of object in the file</typeparam>
        /// <param name="file">File to write the object too</param>
        /// <param name="value">Object to write to the file</param>
        /// <param name="serializer">Serializer used to write the object to the file</param>
        /// <param name="compressor">Optional compressor to the compress the written file</param>
        /// <returns></returns>
        public static Task<Boolean> WriteObjectAsync<T>(
            IFile file,
            T value,
            ISerializer serializer,
            IStreamCompressor compressor = null)
        {
            if (serializer.SupportedModes().HasFlag(SerializationMode.Stream))
                return WriteSerializedStreamAsync<T>(file, value, serializer, compressor);

            return WriteSerializedStringAsync<T>(file, value, serializer, compressor);
        }


        private static Task<Boolean> WriteSerializedStringAsync<T>(
            IFile file,
            T value,
            ISerializer serializer,
            IStreamCompressor compressor = null)
        {
            return Task.Run(async () =>
            {
                if (!(serializer.SupportedModes().HasFlag(SerializationMode.String)))
                    throw new NotSupportedException("The given serializer does not support string serializing");

                try
                {
                    String text = serializer.Serialize(value);

                    // Open the file stream for writing
                    if (compressor == null)
                    {
                        await file.WriteTextAsync(text).ConfigureAwait(false);
                    }
                    else
                    {
                        using (Stream fileStream = await file.OpenWriteAsync().ConfigureAwait(false))
                        using (Stream dataStream = text.AsStream())
                        {
                            fileStream.SetLength(0);
                            // Write the data, compressing with GZip
                            await compressor.CompressToAsync(dataStream, fileStream, CompressionLevel.Optimal).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return false;
                }

                return true;
            });
        }

        private static Task<Boolean> WriteSerializedStreamAsync<T>(
            IFile file,
            T value,
            ISerializer serializer,
            IStreamCompressor compressor = null)
        {
            return Task.Run(async () =>
            {
                if (!(serializer.SupportedModes().HasFlag(SerializationMode.Stream)))
                    throw new NotSupportedException("The given serializer does not support stream serializing");

                try
                {
                    using (Stream fileStream = await file.OpenWriteAsync().ConfigureAwait(false))
                    {
                        fileStream.SetLength(0);
                        if (compressor == null)
                        {
                            serializer.Serialize(value, fileStream);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                // Serialize data into memory stream
                                serializer.Serialize(value, memoryStream);
                                memoryStream.Seek(0, SeekOrigin.Begin);

                                // Write the data, compressing with GZip
                                await compressor.CompressToAsync(memoryStream, fileStream, CompressionLevel.Optimal).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return false;
                }

                return true;
            });
        }

        #endregion




        /// <summary>
        /// Reads the contents of a compressed storage file and returns a byte array representing the decompressed data
        /// </summary>
        /// <param name="file"></param>
        /// <param name="compressor"></param>
        /// <returns></returns>
        public static async Task<Byte[]> ReadCompressedBytesAsync(IFile file, IStreamCompressor compressor)
        {
            byte[] bytes;
            using (Stream stream = await ReadCompressedAsync(file, compressor).ConfigureAwait(false))
            {
                bytes = stream.ToArray();
            }

            return bytes;
        }

        /// <summary>
        /// Reads the contents of a compressed storage file, and return the decompressed data as a memory stream
        /// </summary>
        /// <param name="file"></param>
        /// <param name="compressor"></param>
        /// <returns></returns>
        public static Task<MemoryStream> ReadCompressedAsync(IFile file, IStreamCompressor compressor)
        {
            return ReadCompressedStreamAsync(file, compressor);
        }

        /// <summary>
        /// Reads the contents of a compressed storage file, and return the decompressed data as a memory stream
        /// </summary>
        /// <param name="file"></param>
        /// <param name="compressor"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> ReadCompressedStreamAsync(IFile file, IStreamCompressor compressor)
        {
            // 1. Open the file stream for reading
            using (Stream fileStream = await file.OpenReadAsync().ConfigureAwait(false))
            {
                // 2. Create an InMemoryRandomAccess stream to buffer our data into
                MemoryStream resultStream = new MemoryStream();
                // 3. Decompress the data from the file stream into our buffer stream
                await compressor.DecompressToAsync(resultStream, fileStream).ConfigureAwait(false);
                // 4. Ensure stream position is set to the start
                resultStream.Seek(0, SeekOrigin.Begin);
                // 5. Return our buffer as a basic stream
                return resultStream;
            }
        }

    }
}
