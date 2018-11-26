namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class NetworkClient
    {
        private TcpClient client;
        private IPEndPoint targetEP;
        private Dictionary<int, MessageEvent> messageEvents;

        public NetworkClient(string serverIp, int serverPort)
        {
            client = new TcpClient();
            targetEP = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            messageEvents = new Dictionary<int, MessageEvent>();
        }

        public void Connect()
        {
            try
            {
                client.Connect(targetEP);
                OnConnected();
                Thread receive = new Thread(Receive);
                receive.Start();
            }
            catch (Exception ex)
            {
                Debug.Error(ex.Message);
            }
        }

        public virtual void Register(SenderType sender, MessageType type, MessageEvent @event)
        {
            int key = Message.EnumToKey(sender, type);
            if (messageEvents.ContainsKey(key))
            {
                Debug.Error("不能添加重复key");
                return;
            }
            messageEvents.Add(key, @event);
        }

        public virtual void Send<T>(SenderType sender, MessageType type, T message)
        {
            byte[] data = Message.PackTCPMessage(sender, type, message);
            try
            {
                client.Client.Send(data);
            }
            catch (Exception ex)
            {
                Debug.Error(ex.Message);
            }
        }

        protected abstract void OnConnected();

        protected virtual void Receive()
        {
            while (true)
            {
                try
                {
                    Message.UnpackTCPMessage(client.Client, out SenderType sender, out MessageType type, out byte[] data);
                    int key = Message.EnumToKey(sender, type);
                    if (messageEvents.ContainsKey(key))
                    {
                        MessageEvent @event = messageEvents[key];
                        @event(null, data);
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