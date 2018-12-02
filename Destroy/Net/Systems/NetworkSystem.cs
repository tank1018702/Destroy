namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    internal static class NetworkSystem
    {
        public static NetworkRole Role;

        private static NetServer netServer;
        private static NetClient netClient;

        public static void Init(NetServer server, NetClient client)
        {
            netServer = server;
            netClient = client;

            if (netClient != null && netServer != null)
            {
                Role = NetworkRole.Host;
                netServer.Start();
                netClient.Start();
            }
            else if (netClient != null)
            {
                Role = NetworkRole.Client;
                netClient.Start();
            }
            else if (netServer != null)
            {
                Role = NetworkRole.Server;
                netServer.Start();
            }
        }

        public static void Update(List<GameObject> gameObjects)
        {
            netServer?.Update();
            netClient?.Update();
            
            //foreach (GameObject gameObject in gameObjects)
            //{
            //    if (!gameObject.Active)
            //        continue;
            //    NetworkTransform netTransform = gameObject.GetComponent<NetworkTransform>();
            //    if (!netTransform || !netTransform.Active)
            //        continue;

            //    Transform transform = gameObject.GetComponent<Transform>();
            //    Position position = new Position(transform.Position.X, transform.Position.Y);

            //    byte[] data = Serializer.NetSerialize(position);


            //}
        }
    }

    public abstract class NetServer
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

            public bool SafeSend(out Socket client)
            {
                client = this.client;
                if (client == null || !client.Connected)
                    return false;
                client.Send(data);
                return true;
            }
        }

        private Dictionary<int, CallbackEvent> events;
        private Socket server;
        private Queue<Message> messages;
        bool ready;
        IAsyncResult acceptAsync;
        List<Socket> clients;

        public NetServer(int port)
        {
            events = new Dictionary<int, CallbackEvent>();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(NetworkUtils.LocalIPv4, port));
            messages = new Queue<Message>();
            ready = true;
            acceptAsync = null;
            clients = new List<Socket>();
        }

        public void Register(ushort cmd1, ushort cmd2, CallbackEvent _event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, _event);
        }

        public void Start()
        {
            server.Listen(10); //队列长度
        }

        public void Update()
        {
            if (ready) //异步连接
            {
                acceptAsync = server.BeginAccept(null, null);
                ready = false;
            }
            if (acceptAsync.IsCompleted)
            {
                Socket client = server.EndAccept(acceptAsync); // Try Catch
                clients.Add(client);
                OnConnected(client); //执行回调
                ready = true;
            }

            for (int i = 0; i < clients.Count; i++) //异步读取
            {
                Socket client = clients[i];
                if (!client.Connected)
                {
                    clients.Remove(client);
                    OnDisconnected(client); //执行回调
                }
                else
                {
                    if (client.Available > 0) // client.Poll(1, SelectMode.SelectRead)
                    {
                        //Receive
                        NetworkMessage.UnpackTCPMessage2(client, out ushort cmd1, out ushort cmd2, out byte[] data);
                        int key = NetworkMessage.EnumToKey(cmd1, cmd2);

                        if (events.ContainsKey(key))
                            events[key](client, data); //执行回调
                    }
                }
            }

            //异步发送
            while (messages.Count > 0)
                if (!messages.Dequeue().SafeSend(out Socket client)) //发送失败
                {
                    clients.Remove(client);
                    OnDisconnected(client); //执行回调
                }
        }

        public void Send<T>(Socket client, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(new Message(client, data));
        }

        protected virtual void OnConnected(Socket socket)
        {
        }

        protected virtual void OnDisconnected(Socket socket)
        {
            socket.Close();
        }
    }

    public abstract class NetClient
    {
        public delegate void CallbackEvent(byte[] data);

        private Dictionary<int, CallbackEvent> events;
        private Socket client;
        private Queue<byte[]> messages;
        private string serverIp;
        private int serverPort;

        public NetClient(string serverIp, int serverPort)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            events = new Dictionary<int, CallbackEvent>();
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            messages = new Queue<byte[]>();
        }

        public void Register(ushort cmd1, ushort cmd2, CallbackEvent _event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, _event);
        }

        public void Start()
        {
            client.Connect(new IPEndPoint(IPAddress.Parse(serverIp), serverPort));
            OnConnected(client); //回调方法
        }

        public void Update()
        {
            if (!client.Connected)
            {
                OnDisConnected(client); //执行回调
                return;
            }

            //接受消息
            if (client.Available > 0) //client.Poll(1, SelectMode.SelectRead)
            {
                NetworkMessage.UnpackTCPMessage2(client, out ushort cmd1, out ushort cmd2, out byte[] data);
                int key = NetworkMessage.EnumToKey(cmd1, cmd2);

                if (events.ContainsKey(key))
                    events[key](data); //执行回调
            }

            //发送消息
            while (messages.Count > 0)
            {
                byte[] data = messages.Dequeue();
                if (!client.Connected)
                {
                    OnDisConnected(client);
                    break;
                }
                client.Send(data);
            }
        }

        public void Send<T>(ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(data);
        }

        /// <summary>
        /// 连接完成
        /// </summary>
        protected virtual void OnConnected(Socket client)
        {
        }

        /// <summary>
        /// 连接断开
        /// </summary>
        protected virtual void OnDisConnected(Socket client)
        {
            client.Close();
        }
    }
}