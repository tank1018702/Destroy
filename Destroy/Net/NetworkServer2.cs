namespace Destroy.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class NetworkServer2
    {
        /// <summary>
        /// 线程不安全方法(在方法中对成员字段进行非原子性写入需要加互斥锁)
        /// </summary>
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

        private sealed class Callback
        {
            private CallbackEvent _event;
            private Socket client;
            private byte[] data;

            public Callback(CallbackEvent _event, Socket client, byte[] data)
            {
                this._event = _event;
                this.client = client;
                this.data = data;
            }

            public void Excute() => _event(client, data);
        }

        private Dictionary<int, CallbackEvent> events;
        private Socket server;
        private Thread netThread;
        private ConcurrentQueue<Callback> callbacks;
        private ConcurrentQueue<Message> messages;

        public NetworkServer2(int port)
        {
            events = new Dictionary<int, CallbackEvent>();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(NetworkUtils.LocalIPv4, port));
            netThread = null;
            callbacks = new ConcurrentQueue<Callback>();
            messages = new ConcurrentQueue<Message>();
        }

        /// <summary>
        /// 注册接受消息后调用的方法
        /// </summary>
        public void Register(ushort cmd1, ushort cmd2, CallbackEvent _event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, _event);
        }

        /// <summary>
        /// 开启服务器
        /// </summary>
        public void Start()
        {
            server.Listen(10); //队列长度
            netThread = new Thread(__NetThread) { IsBackground = true };
            netThread.Start();
        }

        /// <summary>
        /// 发送 <see langword="线程安全"/>
        /// </summary>
        public void Send<T>(Socket client, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(new Message(client, data));
        }

        /// <summary>
        /// 单个线程
        /// </summary>
        private void __NetThread()
        {
            bool ready = true;
            IAsyncResult acceptAsync = null;
            List<Socket> clients = new List<Socket>();
            List<Task> tasks = new List<Task>();

            while (true)
            {
                //异步连接
                if (ready)
                {
                    acceptAsync = server.BeginAccept(null, null);
                    ready = false;
                }
                if (acceptAsync.IsCompleted)
                {
                    Socket client = server.EndAccept(acceptAsync); // Try Catch
                    clients.Add(client);
                    OnConnected(client); //回调
                    ready = true;
                }

                //异步读取
                for (int i = 0; i < clients.Count; i++)
                {
                    Socket client = clients[i];
                    if (!client.Connected)
                    {
                        OnDisconnected(client); //执行回调
                        clients.RemoveAt(i);
                    }
                    else
                    {
                        // client.Poll(1, SelectMode.SelectRead)
                        if (client.Available > 0)
                        {
                            Task task = Task.Run(() =>
                            {
                                //Receive
                                NetworkMessage.UnpackTCPMessage2(client, out ushort cmd1, out ushort cmd2, out byte[] data);

                                int key = NetworkMessage.EnumToKey(cmd1, cmd2);
                                if (events.ContainsKey(key))
                                {
                                    CallbackEvent _event = events[key];
                                    Callback callback = new Callback(_event, client, data);
                                    callbacks.Enqueue(callback); //线程安全
                                }
                            });
                            tasks.Add(task);
                        }
                    }
                }
                Task.WaitAll(tasks.ToArray());
                tasks.Clear();

                //处理回调
                while (callbacks.Count > 0)
                    if (callbacks.TryDequeue(out Callback callback))
                        callback.Excute(); //Try Catch

                //异步发送
                while (messages.Count > 0)
                    if (messages.TryDequeue(out Message message))
                        message.Send();

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 客户端上线 <see langword="线程安全"/>
        /// </summary>
        protected virtual void OnConnected(Socket socket) { }

        /// <summary>
        /// 客户端断线 <see langword="线程安全"/>
        /// </summary>
        protected virtual void OnDisconnected(Socket socket) { }
    }
}