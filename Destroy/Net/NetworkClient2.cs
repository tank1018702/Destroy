namespace Destroy.Net
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class NetworkClient2
    {
        /// <summary>
        /// 线程不安全方法(在方法中对成员字段进行非原子性写入需要加互斥锁)
        /// </summary>
        public delegate void CallbackEvent(byte[] data);

        private Dictionary<int, CallbackEvent> events;
        private Socket client;
        private Thread netThread;
        private ConcurrentQueue<byte[]> messages;               //待发送消息

        public NetworkClient2()
        {
            events = new Dictionary<int, CallbackEvent>();
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            netThread = null;
            messages = new ConcurrentQueue<byte[]>();
        }

        /// <summary>
        /// 注册接受消息后调用的方法
        /// </summary>
        public void Register(ushort cmd1, ushort cmd2, CallbackEvent @event)
        {
            int key = NetworkMessage.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, @event);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect(string serverIp, int serverPort)
        {
            client.Connect(new IPEndPoint(IPAddress.Parse(serverIp), serverPort));
            OnConnected(); //回调方法

            netThread = new Thread(__NetThread) { IsBackground = true };
            netThread.Start();
        }

        /// <summary>
        /// 发送 <see langword="线程安全"/>
        /// </summary>
        public void Send<T>(ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkMessage.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(data);
        }

        /// <summary>
        /// 单个线程
        /// </summary>
        private void __NetThread()
        {
            while (true)
            {
                if (!client.Connected)
                {
                    OnDisConnected(); //执行回调
                    client.Close();
                    break;
                }
                //接受消息
                //client.Poll(1, SelectMode.SelectRead)
                if (client.Available > 0)
                {
                    NetworkMessage.UnpackTCPMessage2(client, out ushort cmd1, out ushort cmd2, out byte[] data);
                    int key = NetworkMessage.EnumToKey(cmd1, cmd2);
                    if (events.ContainsKey(key))
                    {
                        CallbackEvent _event = events[key];
                        _event(data); //执行回调
                    }
                }
                
                //发送消息
                while (messages.Count > 0)
                    if (messages.TryDequeue(out byte[] data))
                        client.Send(data); //Try Catch

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 连接完成 <see langword="线程安全"/>
        /// </summary>
        protected virtual void OnConnected() { }

        /// <summary>
        /// 连接断开 <see langword="线程安全"/>
        /// </summary>
        protected virtual void OnDisConnected() { }
    }
}