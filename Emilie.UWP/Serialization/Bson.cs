using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.Framework.Extensions;

namespace Universal.Framework.Serialization
{
    [Windows.Foundation.Metadata.Deprecated("Class not yet implemented", Windows.Foundation.Metadata.DeprecationType.Remove, 1)]
    public class Bson : Instanceable<Bson>, ISerializer
    {

        public SerializationMode SupportedModes()
        {
            return (SerializationMode.String | SerializationMode.Stream);
        }

        /// <summary>
        /// Asynchronously deserializes Bson into the given object type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public Task<T> DeserializeAsync<T>(string bsonString)
        {
            return Task.Run(() =>
            {
                T result = default(T);
                using (var stream = bsonString.AsStream())
                {
                    BsonReader bReader = new BsonReader(stream);
                    JsonSerializer serializer = new JsonSerializer();
                    result = serializer.Deserialize<T>(bReader);
                }
                return result;
            });
        }


        /// <summary>
        /// Asynchronously serializes an object into Json.
        /// Uses custom settings to ignore serializing null values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<String> SerializeAsync<T>(
            T value,
            DefaultValueHandling dvh = DefaultValueHandling.Ignore,
            NullValueHandling nvh = NullValueHandling.Ignore)
        {
            throw new NotImplementedException();

            String result = string.Empty;

            await TaskExtensions.SafeRun(() =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = dvh;
                settings.NullValueHandling = nvh;
                result = JsonConvert.SerializeObject(value, settings);

            }).ConfigureAwait(false);

            return result;
        }

        public async Task<T> DeserializeStreamAsync<T>(System.IO.Stream stream)
        {
            throw new NotImplementedException();
            T result = default(T);

            await TaskExtensions.SafeRun(() =>
            {
                var s = new JsonSerializer();
                s.NullValueHandling = NullValueHandling.Ignore;
                s.DefaultValueHandling = DefaultValueHandling.Ignore;

                using (var reader = new StreamReader(stream))
                {
                    result = (T)s.Deserialize(reader, typeof(T));
                }
            });

            return result;
        }

        public async Task SerializeStreamAsync<T>(T data, System.IO.Stream stream)
        {
            throw new NotImplementedException();
            await TaskExtensions.SafeRun(() =>
            {
                var s = new JsonSerializer();
                s.NullValueHandling = NullValueHandling.Ignore;
                s.DefaultValueHandling = DefaultValueHandling.Ignore;

                using (StreamWriter writer = new StreamWriter(stream))
                using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                {
                    s.Serialize(jsonWriter, data);
                }
            });
        }

        public Task<string> SerializeAsync<T>(T value)
        {
            throw new NotImplementedException();
        }
    }
}
