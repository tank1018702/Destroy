namespace Destroy.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class NetworkServer
    {
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
            private MessageEvent @event;
            private object obj;
            private byte[] data;

            public ServerCallback(MessageEvent @event, object obj, byte[] data)
            {
                this.@event = @event;
                this.obj = obj;
                this.data = data;
            }

            public void Excute() => @event(obj, data);
        }

        private TcpListener listener;                                   //服务器套接字
        private Dictionary<int, MessageEvent> events;                   //注册消息
        private ConcurrentQueue<ServerCallback> callBacks;              //回调事件
        private ConcurrentQueue<ServerMessage> messages;                //待发送消息

        public NetworkServer(int port)
        {
            listener = new TcpListener(NetworkUtils.LocalIPv4, port);
            events = new Dictionary<int, MessageEvent>();
            callBacks = new ConcurrentQueue<ServerCallback>();
            messages = new ConcurrentQueue<ServerMessage>();
        }

        public void Start()
        {
            listener.Start();

            Thread await = new Thread(__Await) { IsBackground = true };
            await.Start();

            Thread handle = new Thread(__Handle) { IsBackground = true };
            handle.Start();
        }

        public void Register(ushort cmd1, ushort cmd2, MessageEvent @event)
        {
            int key = MessagePacker.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, @event);
        }

        protected void Send<T>(TcpClient tcpClient, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = MessagePacker.PackTCPMessage(cmd1, cmd2, message);
            messages.Enqueue(new ServerMessage(tcpClient.GetStream(), data));
        }

        protected virtual void OnAccept(TcpClient tcpClient) { }

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
                if (callBacks.Count > 0)
                    if (callBacks.TryDequeue(out ServerCallback callback))
                        callback.Excute();

                //发送消息
                if (messages.Count > 0)
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
                MessagePacker.UnpackTCPMessage(stream, out ushort cmd1, out ushort cmd2, out byte[] data);
                //执行回调
                OnReceive(tcpClient);

                int key = MessagePacker.EnumToKey(cmd1, cmd2);
                if (events.ContainsKey(key))
                {
                    MessageEvent messageEvent = events[key];
                    ServerCallback callback = new ServerCallback(messageEvent, tcpClient, data);
                    callBacks.Enqueue(callback);
                }
            }
        }
    }
}