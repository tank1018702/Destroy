namespace Destroy.Net
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class NetworkClient
    {
        public delegate void ClientEvent(byte[] data);

        private sealed class ClientCallback
        {
            private ClientEvent _event;
            private byte[] data;

            public ClientCallback(ClientEvent _event, byte[] data)
            {
                this._event = _event;
                this.data = data;
            }

            public void Excute() => _event(data);
        }

        private static Dictionary<int, ClientEvent> events      //注册消息
            = new Dictionary<int, ClientEvent>();

        private TcpClient client;                               //客户端
        private NetworkStream stream;                           //网络流
        private IPEndPoint targetEP;                            //服务器IP端口
        private ConcurrentQueue<ClientCallback> callbacks;      //回调事件
        private ConcurrentQueue<byte[]> messages;               //待发送消息

        private Thread handle;
        private Thread receive;

        public static void Register(ushort cmd1, ushort cmd2, ClientEvent @event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, @event);
        }

        public NetworkClient(string serverIp, int serverPort)
        {
            client = new TcpClient();
            stream = null;
            targetEP = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            callbacks = new ConcurrentQueue<ClientCallback>();
            messages = new ConcurrentQueue<byte[]>();
            handle = null;
            receive = null;
        }

        public void Connect()
        {
            client.Connect(targetEP);
            stream = client.GetStream();
            OnConnect(); //执行回调

            handle = new Thread(__Handle) { IsBackground = true };
            handle.Start();

            receive = new Thread(__Receive) { IsBackground = true };
            receive.Start();
        }

        /// <summary>
        /// 线程安全
        /// </summary>
        protected void Send<T>(ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(data);
        }

        /// <summary>
        /// 线程安全
        /// </summary>
        protected virtual void OnConnect() { }

        /// <summary>
        /// 线程安全
        /// </summary>
        protected virtual void OnReceive() { }

        /// <summary>
        /// 一个线程
        /// </summary>
        private void __Handle()
        {
            while (true)
            {
                //处理回调
                while (callbacks.Count > 0)
                    if (callbacks.TryDequeue(out ClientCallback callback))
                        callback.Excute();

                //发送消息
                while (messages.Count > 0)
                    if (messages.TryDequeue(out byte[] data))
                        stream.Write(data, 0, data.Length);

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 一个线程
        /// </summary>
        private void __Receive()
        {
            while (true)
            {
                NetworkMessage.UnpackTCPMessage(stream, out ushort cmd1, out ushort cmd2, out byte[] data);
                OnReceive(); //执行回调

                int key = NetworkMessage.EnumToKey(cmd1, cmd2);
                if (events.ContainsKey(key))
                {
                    ClientEvent _event = events[key];
                    ClientCallback callback = new ClientCallback(_event, data);
                    callbacks.Enqueue(callback);
                }
            }
        }
    }
}