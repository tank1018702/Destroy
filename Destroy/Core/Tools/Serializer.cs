namespace Destroy
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class Serializer
    {
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

        public static byte[] NetSerialize<T>(T t)
        {
            byte[] data = null;

            //protobuf-net傻逼API设计, 第一次使用Serialize无法填充数组只能获取到长度
            //只能使用两次Serializer.Serialize才能读取出byte
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