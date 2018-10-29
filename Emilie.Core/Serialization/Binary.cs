using Emilie.Core.Extensions;
using Polenter.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Emilie.Core.Serialization
{
    public class Binary : Instanceable<Binary>, ISerializer
    {
        /// <summary>
        /// Stream Serialization
        /// </summary>
        /// <returns></returns>
        
        public SerializationMode SupportedModes()
        {
            return SerializationMode.Stream;
        }

        /// <summary>
        /// STRING MODES NOT SUPPORTED FOR BINARY
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<string> SerializeAsync<T>(T value)
        {
            throw new NotSupportedException("Binary Serializer supports stream modes only");
        }

        /// <summary>
        /// STRING MODES NOT SUPPORTED FOR BINARY
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<T> DeserializeAsync<T>(string data)
        {
            T result = default(T);

            using (var stream = StringExtensions.AsStream(data))
            {
                result = await DeserializeStreamAsync<T>(stream);
            }

            return result;
        }

        public Task<T> DeserializeStreamAsync<T>(Stream stream)
        {
            return Task.Run(() =>
            {
                T output = default(T);
                var serializer = new SharpSerializer(true);
                output = (T)serializer.Deserialize((Stream)stream);
                return output;
            });
        }

        public Task SerializeStreamAsync<T>(T data, Stream stream)
        {
            return Task.Run(() =>
            {
                var settings = new SharpSerializerBinarySettings(BinarySerializationMode.SizeOptimized);
                var serializer = new SharpSerializer(settings);
                serializer.Serialize(data, stream);
            });
        }
    }
}
