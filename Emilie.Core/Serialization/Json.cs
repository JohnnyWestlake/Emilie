using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Emilie.Core.Common;

namespace Emilie.Core.Serialization
{
    public class Json : BasicDefaultable<Json>, ISerializer
    {
        JsonSerializer _defaultSerializer = new JsonSerializer();

        public SerializationMode SupportedModes()
        {
            return (SerializationMode.String | SerializationMode.Stream);
        }




        //------------------------------------------------------
        //
        //  Serialize
        //
        //------------------------------------------------------

        #region Serialize

        public string Serialize<T>(T value)
        {
            return Serialize<T>(value, DefaultValueHandling.Ignore, NullValueHandling.Ignore);
        }

        public String Serialize<T>(
            T value,
            DefaultValueHandling dvh = DefaultValueHandling.Ignore,
            NullValueHandling nvh = NullValueHandling.Ignore)
        {
            String result = string.Empty;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = dvh;
            settings.NullValueHandling = nvh;
            result = JsonConvert.SerializeObject(value, settings);
            settings = null;
            return result;
        }

        public Task<String> SerializeAsync<T>(T value)
        {
            return Task.Run(() => Serialize(value));
        }

        /// <summary>
        /// Asynchronously serializes an object into Json.
        /// Uses custom settings to ignore serializing null values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<String> SerializeAsync<T>(
            T value,
            DefaultValueHandling dvh = DefaultValueHandling.Ignore,
            NullValueHandling nvh = NullValueHandling.Ignore)
        {
            return Task.Run(() => Serialize(value, dvh, nvh));
        }

        public Task SerializeAsync<T>(T data, Stream stream)
        {
            return Task.Run(() => Serialize(data, stream));
        }

        public void Serialize<T>(T data, Stream stream)
        {
            _defaultSerializer.NullValueHandling = NullValueHandling.Ignore;
            _defaultSerializer.DefaultValueHandling = DefaultValueHandling.Ignore;

            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                _defaultSerializer.Serialize(jsonWriter, data);
            }
        }

        #endregion




        //------------------------------------------------------
        //
        //  Deserialize
        //
        //------------------------------------------------------

        #region Deserialize

        public Task<T> DeserializeAsync<T>(Stream stream) => Task.Run(() => Deserialize<T>(stream));

        public Task<T> DeserializeAsync<T>(string json) => Task.Run(() => Deserialize<T>(json));

        public T Deserialize<T>(string json)
        {
            T result = default(T);
            result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public T Deserialize<T>(Stream stream)
        {
            T result = default(T);
            _defaultSerializer.NullValueHandling = NullValueHandling.Ignore;
            _defaultSerializer.DefaultValueHandling = DefaultValueHandling.Ignore;

            using (var reader = new StreamReader(stream))
            {
                result = (T)_defaultSerializer.Deserialize(reader, typeof(T));
            }

            return result;
        }

        #endregion
    }
}
