namespace Destroy.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class NetworkSystem
    {
        private static INet net;

        public static void Init(NetworkRole role, INet net)
        {
            switch (role)
            {
                case NetworkRole.Client:
                    {

                    }
                    break;
                case NetworkRole.Server:
                    {
                        NetworkSystem.net = net;
                    }
                    break;
                case NetworkRole.Host:
                    {

                    }
                    break;
            }
        }

        public static void Update(List<GameObject> gameObjects)
        {
            
            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.Active)
                    continue;
                NetworkTransform netTransform = gameObject.GetComponent<NetworkTransform>();
                if (!netTransform || !netTransform.Active)
                    continue;

                Transform transform = gameObject.GetComponent<Transform>();
                Position position = new Position(transform.Position.X, transform.Position.Y);

                byte[] data = Serializer.NetSerialize(position);


            }
        }
    }

    public interface INet
    {
        void Update();
    }

    public abstract class NetServer : INet
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

            public void Send()
            {
                if (client == null || !client.Connected)
                    return;
                client.Send(data);
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
            server.Listen(10); //队列长度
        }

        public void Register(ushort cmd1, ushort cmd2, CallbackEvent _event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, _event);
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
                    clients.RemoveAt(i);
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
                messages.Dequeue().Send();
        }

        public void Send<T>(Socket client, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(new Message(client, data));
        }

        protected virtual void OnConnected(Socket socket) { }

        protected virtual void OnDisconnected(Socket socket) { }
    }
}