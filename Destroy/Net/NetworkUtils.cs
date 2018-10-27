namespace Destroy.Net
{
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

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

        public static byte[] Serialize(object obj)
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

        public static T Deserialize<T>(byte[] data) where T : class
        {
            if (data == null || !typeof(T).IsSerializable)
                return null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                object obj = formatter.Deserialize(stream);
                return obj as T;
            }
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
    }
}