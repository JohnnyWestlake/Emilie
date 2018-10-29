using Emilie.Core;
using Emilie.Core.Extensions;
using Newtonsoft.Json;
using System;

namespace Emilie.UWP.JsonConverters
{
    /// <summary>
    /// When deserializing the property, it attempts to HTML Decode
    /// the content. Uses <see cref="Emilie.Core.Extensions.StringExtensions.StripHtml"/>
    /// </summary>
    public class HTMLDecodeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader != null)
            {
                try
                {
                    if (objectType == typeof(string[]))
                    {
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            var obj = serializer.Deserialize(reader, objectType);
                            var strs = obj as string[];

                            for (int i = 0; i < strs.Length; i++)
                            {
                                var ins = strs[i].StripHtml();
                                strs[i] = ins;
                            }

                            return strs;
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(reader.Value.ToString()))
                            return String.Empty;

                        var s = reader.Value.ToString().StripHtml();
                        return s.Replace(@"\r\n", " ");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    throw new Exception();
                }
            }

            return "JSON_PARSER_ERROR";
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
