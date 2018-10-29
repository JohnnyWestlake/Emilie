using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Framework.Serialization
{
    /// <summary>
    /// A serializer used for working with Protocol Buffers.
    /// See: https://code.google.com/p/protobuf-net/wiki/GettingStarted
    /// </summary>
    public class Proto : Instanceable<Proto>, ISerializer
    {
        public Task<T> DeserializeAsync<T>(string data)
        {
            throw new NotImplementedException();
        }

        public Task<T> DeserializeStreamAsync<T>(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task<string> SerializeAsync<T>(T value)
        {
            throw new NotImplementedException();
        }

        public Task SerializeStreamAsync<T>(T data, Stream stream)
        {
            throw new NotImplementedException();
        }

        public SerializationMode SupportedModes()
        {
            throw new NotImplementedException();
        }
    }
}
