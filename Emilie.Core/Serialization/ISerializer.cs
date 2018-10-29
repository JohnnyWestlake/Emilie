using System;
using System.IO;
using System.Threading.Tasks;

namespace Emilie.Core.Serialization
{
    public interface ISerializer
    {
        SerializationMode SupportedModes();
        T Deserialize<T>(String data);
        T Deserialize<T>(Stream stream);
        String Serialize<T>(T value);

        Task<T> DeserializeAsync<T>(String data);
        Task<String> SerializeAsync<T>(T value);
        Task<T> DeserializeAsync<T>(Stream stream);
        Task SerializeAsync<T>(T data, Stream stream);
        void Serialize<T>(T data, Stream stream);
    }
}
