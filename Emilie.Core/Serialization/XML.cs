using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Emilie.Core.Serialization
{
    public class XML : BasicDefaultable<XML>, ISerializer
    {
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
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                serializer.Serialize(xmlWriter, value);
            }

            return stringBuilder.ToString();
        }

        public Task<String> SerializeAsync<T>(T value)
        {
            return Task.Run(() => Serialize(value));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Task SerializeAsync<T>(T data, Stream stream)
        {
            return Task.Run(() => Serialize(data, stream));
        }

        public void Serialize<T>(T data, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, data);
        }

        #endregion




        //------------------------------------------------------
        //
        //  Deserialize
        //
        //------------------------------------------------------

        #region Deserialize

        public Task<T> DeserializeAsync<T>(string xml)
        {
            return Task.Run(() =>
            {
                return Deserialize<T>(xml);
            });
        }

        public Task<T> DeserializeAsync<T>(Stream stream)
        {
            return Task.Run(() =>
            {
                return Deserialize<T>(stream);
            });
        }

        public T Deserialize<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T value = default(T);

            using (StringReader stringReader = new StringReader(xml))
            {
                object deserialized = serializer.Deserialize(stringReader);
                value = (T)deserialized;
            }

            return value;
        }

        public T Deserialize<T>(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        #endregion
    }
}
