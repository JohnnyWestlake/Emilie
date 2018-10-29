//using Newtonsoft.Dson;
////using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Universal.Framework.Serialization
//{
//    public class Dson : Instanceable<Dson>, ISerializer
//    {
//        public SerializationMode SupportedModes()
//        {
//            return (SerializationMode.String);
//        }

//        /// <summary>
//        /// Asynchronously deserializes Json into the given object type
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="json"></param>
//        /// <returns></returns>
//        public Task<T> DeserializeAsync<T>(string json)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<String> SerializeAsync<T>(T value)
//        {
//            return Task.Run(() =>
//            {
//                //JsonSerializerSettings settings = new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore };
//                return DsonConvert.SerializeObject(value);
//            });
//        }

//        public Task<T> DeserializeStreamAsync<T>(System.IO.Stream stream)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SerializeStreamAsync<T>(T data, System.IO.Stream stream)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
