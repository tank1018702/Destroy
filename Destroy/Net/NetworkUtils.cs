namespace Destroy.Net
{
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using ProtoBuf;

    public static class NetworkUtils
    {
        public static long Ping(IPEndPoint ip)
        {
            UdpClient client = new UdpClient();

            client.Connect(ip);

            Stopwatch watch = Stopwatch.StartNew();

            client.Send(new byte[] { 0 }, 1);
            var data = client.Receive(ref ip);

            long millis = watch.Elapsed.Milliseconds;
            watch.Stop();

            client.Close();

            return millis;
        }

        public static string GetLocalIPv4()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry iPEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < iPEntry.AddressList.Length; i++)
            {
                if (iPEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    return iPEntry.AddressList[i].ToString();
            }
            return null;
        }

        public static byte[] NetSerialize<T>(T t)
        {
            byte[] data = null;

            //protobuf-net傻逼API设计, 第一次使用Serialize无法填充数组只能获取到长度
            //只能使用两次Serializer.Serialize才能读取出byte
            using (Stream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, t);
                data = new byte[stream.Length];

                MemoryStream memoryStream = new MemoryStream(data);
                Serializer.Serialize(memoryStream, t);
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.Read(data, 0, data.Length);
            }
            return data;
        }

        public static T NetDeserialize<T>(byte[] data)
        {
            using (Stream stream = new MemoryStream(data))
            {
                T t = Serializer.Deserialize<T>(stream);
                return t;
            }
        }
    }
}