namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public class NetworkClient
    {
        public delegate void CallbackEvent(byte[] data);

        public bool Connected { get; private set; }
        private readonly string serverIp;
        private readonly int serverPort;
        private Dictionary<int, CallbackEvent> events;
        private Socket client;
        private Queue<byte[]> messages;

        public NetworkClient(string serverIp, int serverPort)
        {
            Connected = false;
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            events = new Dictionary<int, CallbackEvent>();
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            messages = new Queue<byte[]>();
        }

        /// <summary>
        /// 连接完成
        /// </summary>
        public event Action<Socket> OnConnected;

        /// <summary>
        /// 连接断开
        /// </summary>
        public event Action<Socket> OnDisConnected;

        public void Register(ushort cmd1, ushort cmd2, CallbackEvent _event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, _event);
        }

        public void Send<T>(ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(data);
        }

        internal void Start()
        {
            //可能导致异常
            client.Connect(new IPEndPoint(IPAddress.Parse(serverIp), serverPort));

            Connected = true;
            OnConnected?.Invoke(client);
        }

        internal void Handle()
        {
            if (!Connected)
                return;
            //接受消息
            if (client.Available > 0) //client.Poll(1, SelectMode.SelectRead)
            {
                try
                {
                    NetworkMessage.UnpackTCPMessage(client, out ushort cmd1, out ushort cmd2, out byte[] data);
                    int key = NetworkMessage.EnumToKey(cmd1, cmd2);

                    if (events.ContainsKey(key))
                        events[key](data);
                }
                catch (Exception)
                {
                    client.Close();
                    Connected = false;
                    OnDisConnected?.Invoke(client);
                    return;
                }
            }

            //发送消息
            while (messages.Count > 0)
            {
                byte[] data = messages.Dequeue();
                try
                {
                    client.Send(data);
                }
                catch (Exception)
                {
                    client.Close();
                    Connected = false;
                    OnDisConnected?.Invoke(client);
                    return;
                }
            }
        }
    }
}