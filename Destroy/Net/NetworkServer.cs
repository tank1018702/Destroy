namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public abstract class NetworkServer
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
        private bool ready;
        private IAsyncResult acceptAsync;
        private List<Socket> clients;

        public NetworkServer(int port)
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

        public void Send<T>(Socket client, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(new Message(client, data));
        }

        public void Start()
        {
            server.Listen(10); //队列长度
        }

        public void Handle()
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

        protected virtual void OnConnected(Socket socket)
        {
        }

        protected virtual void OnDisconnected(Socket socket)
        {
            socket.Close();
        }
    }
}
