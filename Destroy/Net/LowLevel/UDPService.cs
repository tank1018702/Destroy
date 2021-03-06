﻿namespace Destroy.Net
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public class UDPService
    {
        public delegate void CallbackEvent(byte[] data);

        private sealed class Message
        {
            private IPEndPoint endPoint;
            private byte[] data;

            public Message(IPEndPoint endPoint, byte[] data)
            {
                this.endPoint = endPoint;
                this.data = data;
            }

            public void Send(UdpClient udp) => udp.Send(data, data.Length, endPoint);
        }

        private Dictionary<int, CallbackEvent> events;
        private UdpClient udp;
        private Queue<Message> messages;

        public UDPService(string ip, int port)
        {
            events = new Dictionary<int, CallbackEvent>();
            udp = new UdpClient(new IPEndPoint(IPAddress.Parse(ip), port));
            messages = new Queue<Message>();
        }

        public void Register(ushort cmd1, ushort cmd2, CallbackEvent _event)
        {
            int key = NetworkSerializer.EnumToKey(cmd1, cmd2);
            if (events.ContainsKey(key))
                return;
            events.Add(key, _event);
        }

        public void Send<T>(string ip, int port, ushort cmd1, ushort cmd2, T message)
        {
            byte[] data = NetworkSerializer.PackUDPMessage(cmd1, cmd2, message);
            messages.Enqueue(new Message(new IPEndPoint(IPAddress.Parse(ip), port), data));
        }

        /// <summary>
        /// 局域网广播
        /// </summary>
        public void Broadcast<T>(int targetPort, ushort cmd1, ushort cmd2, T message)
        {
            Send(IPAddress.Broadcast.ToString(), targetPort, cmd1, cmd2, message);
        }

        public void Update()
        {
            //接收消息
            if (udp.Available > 0)
            {
                IPEndPoint iPEndPoint = null;
                byte[] data = udp.Receive(ref iPEndPoint);

                NetworkSerializer.UnpackUDPMessage(data, out ushort cmd1, out ushort cmd2, out byte[] msgData);
                int key = NetworkSerializer.EnumToKey(cmd1, cmd2);

                if (events.ContainsKey(key))
                    events[key](msgData);
            }

            //发送消息
            while (messages.Count > 0)
            {
                Message message = messages.Dequeue();
                message.Send(udp);
            }
        }
    }
}