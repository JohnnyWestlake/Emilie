using Emilie.Core;
using Emilie.Core.Serialization;
using Polenter.Serialization;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Emilie.UWP.Serialization
{
    public class Binary : BasicDefaultable<Binary>, ISerializer
    {
        public readonly SharpSerializer Serializer = null;

        public Binary()
        {
            SharpSerializerBinarySettings settings = new SharpSerializerBinarySettings(BinarySerializationMode.SizeOptimized);
            Serializer = new SharpSerializer(settings);
            Serializer.PropertyProvider.AttributesToIgnore.Add(typeof(IgnoreDataMemberAttribute));
        }

        /// <summary>
        /// Stream Serialization
        /// </summary>
        /// <returns></returns>

        public SerializationMode SupportedModes()
        {
            return SerializationMode.Stream;
        }





        //------------------------------------------------------
        //
        //  Serialize
        //
        //------------------------------------------------------

        #region Serialize

        /// <summary>
        /// STRING MODES NOT SUPPORTED FOR BINARY
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Serialize<T>(T value)
        {
            throw new NotSupportedException("Binary Serializer supports stream modes only");
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

        public Task SerializeAsync<T>(T data, Stream stream)
        {
            return Task.Run(() => Serializer.Serialize(data, stream));
        }

        public void Serialize<T>(T data, Stream stream)
        {
            Serializer.Serialize(data, stream);
        }

        #endregion






        //------------------------------------------------------
        //
        //  Deserialize
        //
        //------------------------------------------------------

        #region Deserialize

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return Task.Run(() =>
            {
                return Deserialize<T>(stream);
            });
        }

        public T Deserialize<T>(Stream stream)
        {
            T output = default(T);
            output = (T)Serializer.Deserialize((Stream)stream);
            return output;
        }



        public Task<T> DeserializeAsync<T>(string data)
        {
            return Task.Run(() =>
            {
                return Deserialize<T>(data);
            });
        }

        public T Deserialize<T>(string data)
        {
            T result = default(T);

            using (var stream = Encoding.UTF8.GetBytes(data).AsBuffer().AsStream())
            {
                result = Deserialize<T>(stream);
            }

            return result;
        }

        #endregion

    }
}
