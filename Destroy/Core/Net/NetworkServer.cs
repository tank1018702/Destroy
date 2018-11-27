namespace Destroy.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;

    public class ServerCallback
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

    public abstract class NetworkServer
    {
        private TcpListener listener;                                   //服务器套接字
        private Dictionary<int, MessageEvent> messageEvents;            //注册消息
        private ConcurrentQueue<ServerCallback> callBacks;              //回调事件

        public NetworkServer(int port)
        {
            listener = new TcpListener(NetworkUtils.LocalIPv4, port);
            messageEvents = new Dictionary<int, MessageEvent>();
            callBacks = new ConcurrentQueue<ServerCallback>();
        }

        public void Start()
        {
            Await();
            Handle();
        }

        public void Register(ushort cmd1, ushort cmd2, MessageEvent @event)
        {
            int key = Message.EnumToKey(cmd1, cmd2);
            if (messageEvents.ContainsKey(key))
                return;
            messageEvents.Add(key, @event);
        }

        protected void Send<T>(TcpClient tcpClient, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = Message.PackTCPMessage(cmd1, cmd2, message);
            try
            {
                tcpClient.Client.Send(data);
            }
            catch (Exception ex)
            {
                Debug.Error(ex.Message);
            }
        }

        protected virtual object OnAccept(TcpClient tcpClient) { return null; }

        private void Await()
        {
            listener.Start();

            Thread await = new Thread(_Await) { IsBackground = true };
            await.Start();

            void _Await()
            {
                while (true)
                {
                    try
                    {
                        TcpClient tcpClient = listener.AcceptTcpClient();
                        object obj = OnAccept(tcpClient);
                        KeyValuePair<TcpClient, object> pair = new KeyValuePair<TcpClient, object>(tcpClient, obj);

                        Thread receive = new Thread(Receive) { IsBackground = true };
                        receive.Start(pair);
                    }
                    catch (Exception ex)
                    {
                        Debug.Error(ex.Message);
                    }
                }
            }
        }

        private void Handle()
        {
            Thread handle = new Thread(_Handle) { IsBackground = true };
            handle.Start();

            void _Handle()
            {
                while (true)
                {
                    if (callBacks.Count > 0)
                        if (callBacks.TryDequeue(out ServerCallback callback))
                            callback.Excute();
                    Thread.Sleep(1);
                }
            }
        }

        private void Receive(object param)
        {
            var pair = (KeyValuePair<TcpClient, object>)param;
            Socket socket = pair.Key.Client;
            object obj = pair.Value;

            while (true)
            {
                try
                {
                    Message.UnpackTCPMessage(socket, out ushort cmd1, out ushort cmd2, out byte[] data);
                    int key = Message.EnumToKey(cmd1, cmd2);
                    if (messageEvents.ContainsKey(key))
                    {
                        MessageEvent @event = messageEvents[key];
                        ServerCallback callback = new ServerCallback(@event, obj, data);
                        callBacks.Enqueue(callback);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Error(ex.Message);
                }
            }
        }
    }
}