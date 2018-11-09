namespace Destroy
{
    using System.IO;

    public static class Storage
    {
        public static void SaveToJson<T>(T obj, string path) where T : new()
        {
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] data = Serializer.JsonSerialize(obj);
                //清空之前文件
                stream.SetLength(0); //???
                stream.Write(data, 0, data.Length);
            }
        }

        public static T LoadFromJson<T>(string path) where T : new()
        {
            byte[] data = new byte[65536]; //最大65KB

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int length = stream.Read(data, 0, data.Length);
                T obj = Serializer.JsonDeserialize<T>(data, 0, length);
                return obj;
            }
        }
    }
}