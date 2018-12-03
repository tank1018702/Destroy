namespace Destroy
{
    using LitJson;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

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

    public static partial class Serializer
    {
        /// <summary>
        /// 如果不想序列化类中某个字段或属性使用 [NonSerializable]
        /// </summary>
        public static byte[] NativeSerialize(object obj)
        {
            if (obj == null || !obj.GetType().IsSerializable)
                return null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                byte[] data = stream.ToArray();
                return data;
            }
        }

        public static byte[] JsonSerialize<T>(T obj)
        {
            string json = JsonMapper.ToJson(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            return data;
        }

        public static byte[] NetSerialize<T>(T t)
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

        public static T NativeDeserialize<T>(byte[] data)
        {
            if (data == null || !typeof(T).IsSerializable)
                return default(T);
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                object obj = formatter.Deserialize(stream);
                return (T)obj;
            }
        }

        public static T JsonDeserialize<T>(byte[] data, int index, int count) where T : new()
        {
            string json = Encoding.UTF8.GetString(data, index, count);
            T obj = JsonMapper.ToObject<T>(json);  //反序列化时必须保证类型拥有无参构造
            return obj;
        }

        public static T NetDeserialize<T>(byte[] data)
        {
            using (Stream stream = new MemoryStream(data))
            {
                T t = ProtoBuf.Serializer.Deserialize<T>(stream);
                return t;
            }
        }
    }
}