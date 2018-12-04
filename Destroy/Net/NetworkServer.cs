namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public class NetworkServer
    {
        public delegate void CallbackEvent(Socket client, byte[] data);

        private sealed class Message
        {
            private Socket client;
            private byte[] data;

            public Message(Socket client, byte[] data)
            {
                this.client = client;
                this.data = data;
            }

            public void Send(out Socket client)
            {
                client = this.client;
                client.Send(data);
            }
        }

        private sealed class Client
        {
            public bool Connected;
            public Socket Socket;

            public Client(bool connected,Socket socket)
            {
                Connected = connected;
                Socket = socket;
            }
        }

        private Dictionary<int, CallbackEvent> events;
        private Socket server;
        private Queue<Message> messages;
        private bool accept;
        private IAsyncResult acceptAsync;
        private List<Socket> clients;

        public NetworkServer(int port)
        {
            events = new Dictionary<int, CallbackEvent>();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(NetworkUtils.LocalIPv4, port));
            messages = new Queue<Message>();
            accept = true;
            acceptAsync = null;
            clients = new List<Socket>();
        }

        /// <summary>
        /// 收到客户端连接
        /// </summary>
        public event Action<Socket> OnConnected;

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        public event Action<Socket> OnDisconnected;

        public void Register(ushort cmd1, ushort cmd2, CallbackEvent _event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, _event);
        }

        public void Send<T>(Socket client, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(new Message(client, data));
        }

        internal void Start() => server.Listen(10); //队列长度

        internal void Handle()
        {
            //异步接收
            if (accept)
            {
                try
                {
                    acceptAsync = server.BeginAccept(null, null);
                    accept = false;
                }
                catch (Exception) { }
            }
            if (acceptAsync.IsCompleted)
            {
                try
                {
                    Socket client = server.EndAccept(acceptAsync);
                    clients.Add(client);
                    OnConnected?.Invoke(client);
                }
                catch (Exception) { }
                finally { accept = true; }
            }

            //异步读取
            List<Socket> rmSockets = new List<Socket>();
            foreach (Socket client in clients)
            {
                if (client.Available > 0) // client.Poll(1, SelectMode.SelectRead)
                {
                    try
                    {
                        NetworkMessage.UnpackTCPMessage(client, out ushort cmd1, out ushort cmd2, out byte[] data);
                        int key = NetworkMessage.EnumToKey(cmd1, cmd2);

                        if (events.ContainsKey(key))
                            events[key](client, data);
                    }
                    catch (Exception)
                    {
                        client.Close();
                        rmSockets.Add(client);
                        OnDisconnected?.Invoke(client);
                    }
                }
            }
            rmSockets.ForEach(socket => clients.Remove(socket)); //移除错误套接字

            //异步发送
            while (messages.Count > 0)
            {
                Message message = messages.Dequeue();
                Socket client = null;
                try
                {
                    message.Send(out client);
                }
                catch (Exception)
                {
                    client.Close();
                    clients.Remove(client);
                    OnDisconnected?.Invoke(client);
                    break;
                }
            }
        }
    }
}