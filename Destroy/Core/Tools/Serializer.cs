﻿namespace Destroy
{
    using LitJson;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

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

        public static byte[] JsonSerialize<T>(T obj)
        {
            string json = JsonMapper.ToJson(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            return data;
        }

        public static T JsonDeserialize<T>(byte[] data, int index, int count) where T : new()
        {
            string json = Encoding.UTF8.GetString(data, index, count);
            T obj = JsonMapper.ToObject<T>(json);  //反序列化时必须保证类型拥有无参构造
            return obj;
        }
    }
}