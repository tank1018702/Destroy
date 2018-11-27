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

        public void Register(ushort cmd1, ushort cmd2, MessageEvent @event)
        {
            int key = Message.EnumToKey(cmd1, cmd2);
            if (messageEvents.ContainsKey(key))
            {
                Debug.Error("不能添加重复key");
                return;
            }
            messageEvents.Add(key, @event);
        }

        protected void Send<T>(ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = Message.PackTCPMessage(cmd1, cmd2, message);
            try
            {
                client.Client.Send(data);
            }
            catch (Exception ex)
            {
                Debug.Error(ex.Message);
            }
        }

        protected virtual void OnConnected() { }

        private void Receive()
        {
            while (true)
            {
                try
                {
                    Message.UnpackTCPMessage(client.Client, out ushort cmd1, out ushort cmd2, out byte[] data);
                    int key = Message.EnumToKey(cmd1, cmd2);
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