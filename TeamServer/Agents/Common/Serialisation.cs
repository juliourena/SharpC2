using System.IO;
using System.Runtime.Serialization.Json;

namespace Common
{
    public class Serialisation
    {
        public static byte[] SerialiseData<T>(T data)
        {
            using (var ms = new MemoryStream())
            {
                var serialiser = new DataContractJsonSerializer(typeof(T));
                serialiser.WriteObject(ms, data);
                return ms.ToArray();
            }
        }

        public static T DeserialiseData<T>(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var serialiser = new DataContractJsonSerializer(typeof(T));
                return (T)serialiser.ReadObject(ms);
            }
        }
    }
}
