using System;
using System.Linq;
using Newtonsoft.Json;

namespace Emilie.Core.Serialization.JsonConverters
{
    /// <summary>
    /// Converter to handle when an API returns an array of something which should be a single item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// Plus, any excuse to use the term "Singularity"
    public class MultiplicityToSingularityConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader != null)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    var obj = serializer.Deserialize(reader, typeof(T[]));
                    T[] items = obj as T[];
                    return items.FirstOrDefault();
                }
                else
                {
                    var obj = serializer.Deserialize(reader, typeof(T));
                    T item = (T)obj;
                    return item;
                }
            }

            throw new JsonSerializationException("Json serializer null");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("Cannot serialise single into an array (yet)");
        }
    }

    /// <summary>
    /// Converter to handle when an API returns an single item of something which should be an array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// Plus, any excuse to use the term "Singularity"
    public class SingularityToMultiplicityConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader != null)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    var obj = serializer.Deserialize(reader, typeof(T[]));
                    T[] items = obj as T[];
                    return items;
                }
                else
                {
                    var obj = serializer.Deserialize(reader, typeof(T));
                    T item = (T)obj;
                    return new T[1] { item };
                }
            }

            throw new JsonSerializationException("Json serializer null");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("Cannot serialise array into a single (yet)");
        }
    }
}
