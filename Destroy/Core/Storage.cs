using System.IO;
using System.Text;
using LitJson;

namespace Destroy
{
    public static class Storage
    {
        public static void Save<T>(T obj, string path) where T : new()
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

        public static T Load<T>(string path) where T : new()
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
    }
}