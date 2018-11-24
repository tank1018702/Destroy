namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    //public interface NetworkClient
    //{
    //    void Connect();
    //    void Send();
    //    void Receive();
    //    void Close();
    //}
    //public interface NetworkServer
    //{
    //    void Accept();
    //    void Broadcast();
    //    void Receive();
    //    void Close();
    //}

    public enum SenderType : ushort
    {
        Server,
        Client
    }

    public enum MessageType : ushort
    {

    }

    public delegate void MessageEvent(object obj, byte[] data);

    public class VirtualNetworkClient
    {
        public int Id;
        public TcpClient Client;
        public NetworkStream Stream;

        public VirtualNetworkClient(int id, TcpClient client, NetworkStream stream)
        {
            Id = id;
            Client = client;
            Stream = stream;
        }
    }

    public class NetworkServer
    {
        private int clientId;
        private TcpListener listener;                                   //服务器套接字
        private Dictionary<int, VirtualNetworkClient> virtualClients;   //虚拟客户端
        private Dictionary<int, MessageEvent> messageEvents;            //消息回调

        public NetworkServer(int port)
        {
            clientId = 0;
            listener = new TcpListener(NetworkUtils.LocalIPv4, port);
            virtualClients = new Dictionary<int, VirtualNetworkClient>();
            messageEvents = new Dictionary<int, MessageEvent>();
        }

        public void Await()
        {
            listener.Start();
            TcpClient client = null;
            NetworkStream stream = null;

            while (true)
            {
                try
                {
                    IAsyncResult async = listener.BeginAcceptSocket(null, null);
                    while (!async.IsCompleted)
                        Thread.Sleep(1);
                    client = listener.EndAcceptTcpClient(async);
                    stream = client.GetStream();
                    VirtualNetworkClient virtualNetworkClient = new VirtualNetworkClient(clientId++, client, stream);
                }
                catch (Exception ex)
                {
                    Debug.Error(ex.Message);
                }
            }
        }

        public void Register(SenderType sender, MessageType type, MessageEvent @event)
        {
            if (messageEvents == null)
                messageEvents = new Dictionary<int, MessageEvent>();
            int key = Message.EnumToKey(sender, type);
            if (messageEvents.ContainsKey(key))
            {
                Debug.Error("不能添加重复key");
                return;
            }
            messageEvents.Add(key, @event);
        }
    }

    public static class Message
    {
        public static int EnumToKey(SenderType sender, MessageType type)
        {
            ushort temp = (ushort)((ushort)sender << 8);
            ushort key = (ushort)(temp + (ushort)type);
            return key;
        }

        /// <summary>
        /// 把发送者类型, 消息类型, 消息打成一个包
        /// </summary>
        public static byte[] PackTCPMessage<T>(SenderType sender, MessageType type, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes((ushort)sender);
            byte[] typeData = BitConverter.GetBytes((ushort)type);
            //使用Protobuf-net
            byte[] data = Serializer.NetSerialize(message);
            byte[] bodyLen = BitConverter.GetBytes
                ((ushort)(senderData.Length + typeData.Length + data.Length));

            //packet head
            datas.AddRange(bodyLen);     // 2bytes (the length of the packet body)
            //packet body
            datas.AddRange(senderData);  // 2bytes
            datas.AddRange(typeData);    // 2bytes
            datas.AddRange(data);        // nbytes

            return datas.ToArray();
        }

        /// <summary>
        /// 利用Socket读取一个打好的包
        /// </summary>
        public static void UnpackTCPMessage<T>(Socket socket, out SenderType sender, out MessageType type, out T message)
        {
            ushort bodyLen;
            byte[] head = new byte[2];
            socket.Receive(head);

            bodyLen = BitConverter.ToUInt16(head, 0);            // 2bytes (the length of the packet body)
            byte[] body = new byte[bodyLen];
            socket.Receive(body);

            using (MemoryStream stream = new MemoryStream(body))
            {
                BinaryReader reader = new BinaryReader(stream);
                sender = (SenderType)reader.ReadUInt16();        // 2bytes
                type = (MessageType)reader.ReadUInt16();         // 2bytes
                byte[] data = reader.ReadBytes(bodyLen - 4);     // nbytes
                //使用Protobuf-net
                message = Serializer.NetDeserialize<T>(data);
            }
        }

        public static byte[] PackUDPMessage<T>(SenderType sender, MessageType type, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes((ushort)sender);
            byte[] typeData = BitConverter.GetBytes((ushort)type);
            //使用Protobuf-net
            byte[] data = Serializer.NetSerialize<T>(message);

            //packet
            datas.AddRange(senderData); // 2bytes
            datas.AddRange(typeData);   // 2bytes
            datas.AddRange(data);       // nbytes

            return datas.ToArray();
        }

        public static void UnpackUDPMessage<T>(byte[] data, out SenderType sender, out MessageType type, out T message)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);
                sender = (SenderType)reader.ReadUInt16();  // 2bytes
                type = (MessageType)reader.ReadUInt16();
                byte[] msgData = reader.ReadBytes(data.Length - 4);
                //使用Protobuf-net
                message = Serializer.NetDeserialize<T>(msgData);
            }
        }
    }
}