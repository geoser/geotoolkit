using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GeoToolkit.Serialization
{
    public static class BinaryFormatterHelper
    {
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                return default(T);

            using (var stream = new MemoryStream(data))
            {
                IFormatter formatter = new BinaryFormatter();
                return (T) formatter.Deserialize(stream);
            }
        }
    }
}