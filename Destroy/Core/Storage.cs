namespace Destroy
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using LitJson;

    public static class Storage
    {
        public static void SaveToJson<T>(T obj, string path) where T : new()
        {
            string json = JsonMapper.ToJson(obj);

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] data = Encoding.UTF8.GetBytes(json); //UTF8
                //清空之前文件
                stream.SetLength(0);
                stream.Write(data, 0, data.Length);
            }
        }

        public static T LoadFromJson<T>(string path) where T : new()
        {
            byte[] data = new byte[65536]; //最大65KB

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int length = stream.Read(data, 0, data.Length);
                string json = Encoding.UTF8.GetString(data, 0, length); //UTF8
                T obj = JsonMapper.ToObject<T>(json); //反序列化时必须保证类型拥有无参构造
                return obj;
            }
        }

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
    }
}