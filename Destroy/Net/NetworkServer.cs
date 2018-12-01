namespace Destroy.Net
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class NetworkServer
    {
        public delegate void ServerEvent(object obj, byte[] data);

        private sealed class ServerMessage
        {
            private NetworkStream stream;
            private byte[] data;

            public ServerMessage(NetworkStream stream, byte[] data)
            {
                this.stream = stream;
                this.data = data;
            }

            public void Send() => stream.Write(data, 0, data.Length);
        }

        private sealed class ServerCallback
        {
            private ServerEvent _event;
            private object obj;
            private byte[] data;

            public ServerCallback(ServerEvent _event, object obj, byte[] data)
            {
                this._event = _event;
                this.obj = obj;
                this.data = data;
            }

            public void Excute() => _event(obj, data);
        }

        private static Dictionary<int, ServerEvent> events              //注册消息
            = new Dictionary<int, ServerEvent>();

        private TcpListener listener;                                   //服务器
        private ConcurrentQueue<ServerCallback> callbacks;              //回调事件(回调事件会在一个特定线程中执行, 如果需要修改gameThread的全局数据建议在方法中使用lock)
        private ConcurrentQueue<ServerMessage> messages;                //待发送消息

        private Thread await;
        private Thread handle;
        private List<Thread> receives;

        public static void Register(ushort cmd1, ushort cmd2, ServerEvent @event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, @event);
        }

        public NetworkServer(int port)
        {
            listener = new TcpListener(NetworkUtils.LocalIPv4, port);
            callbacks = new ConcurrentQueue<ServerCallback>();
            messages = new ConcurrentQueue<ServerMessage>();
            await = null;
            handle = null;
            receives = new List<Thread>();
        }

        public void Start()
        {
            listener.Start();

            await = new Thread(__Await) { IsBackground = true };
            await.Start();

            handle = new Thread(__Handle) { IsBackground = true };
            handle.Start();
        }

        /// <summary>
        /// 线程安全
        /// </summary>
        protected void Send<T>(TcpClient tcpClient, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(new ServerMessage(tcpClient.GetStream(), data));
        }

        /// <summary>
        /// 线程安全
        /// </summary>
        protected virtual void OnAccept(TcpClient tcpClient) { }

        /// <summary>
        /// 线程不安全
        /// </summary>
        protected virtual void OnReceive(TcpClient tcpClient) { }

        /// <summary>
        /// 一个线程
        /// </summary>
        private void __Await()
        {
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                OnAccept(tcpClient); //执行回调

                Thread receive = new Thread(__Receive) { IsBackground = true };
                receive.Start(tcpClient);
                receives.Add(receive);
            }
        }

        /// <summary>
        /// 一个线程
        /// </summary>
        private void __Handle()
        {
            while (true)
            {
                //处理回调
                while (callbacks.Count > 0)
                    if (callbacks.TryDequeue(out ServerCallback callback))
                        callback.Excute();

                //发送消息
                while (messages.Count > 0)
                    if (messages.TryDequeue(out ServerMessage message))
                        message.Send();

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 多个线程
        /// </summary>
        private void __Receive(object param)
        {
            TcpClient tcpClient = (TcpClient)param;
            NetworkStream stream = tcpClient.GetStream();

            while (true)
            {
                NetworkMessage.UnpackTCPMessage(stream, out ushort cmd1, out ushort cmd2, out byte[] data);
                OnReceive(tcpClient); //执行回调

                int key = NetworkMessage.EnumToKey(cmd1, cmd2);
                if (events.ContainsKey(key))
                {
                    ServerEvent _event = events[key];
                    ServerCallback callback = new ServerCallback(_event, tcpClient, data);
                    callbacks.Enqueue(callback);
                }
            }
        }
    }
}