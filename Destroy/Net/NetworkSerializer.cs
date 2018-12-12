namespace Destroy.Net
{
    using System.IO;

#if Protobuf
    using Google.Protobuf;

    public static partial class Serializer
    {
        public static byte[] ProtoSerializer<T>(T obj) where T : Google.Protobuf.IMessage
        {
            byte[] data = obj.ToByteArray();
            return data;
        }

        public static T ProtoDeserializer<T>(byte[] data) where T : Google.Protobuf.IMessage, new()
        {
            IMessage message = new T();
            T msg = (T)message.Descriptor.Parser.ParseFrom(data);
            return msg;
        }
    }
#endif

    public static class NetworkSerializer
    {
        public static byte[] Serialize<T>(T t)
        {
            byte[] data = null;

            using (Stream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, t);
                data = new byte[stream.Length];

                MemoryStream memoryStream = new MemoryStream(data);
                ProtoBuf.Serializer.Serialize(memoryStream, t);
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.Read(data, 0, data.Length);
            }
            return data;
        }

        public static T Deserialize<T>(byte[] data) where T : new()
        {
            using (Stream stream = new MemoryStream(data))
            {
                T t = ProtoBuf.Serializer.Deserialize<T>(stream); //反序列化时必须保证类型拥有无参构造
                return t;
            }
        }
    }
}