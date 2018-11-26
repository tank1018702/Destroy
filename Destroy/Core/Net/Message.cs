namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using ProtoBuf;

    public enum SenderType : ushort
    {
        Server,
        Client
    }

    public enum MessageType : ushort
    {
        Hello
    }

    [ProtoContract]
    public class Hello
    {
        [ProtoMember(1)]
        public string Msg;
    }

    public delegate void MessageEvent(object obj, byte[] data);

    public static class Message
    {
        public static int EnumToKey(SenderType sender, MessageType type)
        {
            ushort temp = (ushort)((ushort)sender << 8);
            ushort key = (ushort)(temp + (ushort)type);
            return key;
        }

        /// <summary>
        /// 打TCP包
        /// </summary>
        public static byte[] PackTCPMessage<T>(SenderType sender, MessageType type, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes((ushort)sender);
            byte[] typeData = BitConverter.GetBytes((ushort)type);
            //使用Protobuf-net
            byte[] data = Destroy.Serializer.NetSerialize(message);
            byte[] bodyLen = BitConverter.GetBytes((ushort)(senderData.Length + typeData.Length + data.Length));

            //packet head
            datas.AddRange(bodyLen);     // 2bytes (the length of the packet body)
            //packet body
            datas.AddRange(senderData);  // 2bytes
            datas.AddRange(typeData);    // 2bytes
            datas.AddRange(data);        // nbytes

            return datas.ToArray();
        }

        /// <summary>
        /// 解TCP包
        /// </summary>
        public static void UnpackTCPMessage(Socket socket, out SenderType sender, out MessageType type, out byte[] data)
        {
            ushort bodyLen;
            byte[] head = new byte[2];
            socket.Receive(head);

            bodyLen = BitConverter.ToUInt16(head, 0);           // 2bytes (the length of the packet body)
            byte[] body = new byte[bodyLen];
            socket.Receive(body);

            using (MemoryStream stream = new MemoryStream(body))
            {
                BinaryReader reader = new BinaryReader(stream);
                sender = (SenderType)reader.ReadUInt16();       // 2bytes
                type = (MessageType)reader.ReadUInt16();        // 2bytes
                data = reader.ReadBytes(bodyLen - 4);           // nbytes
            }
        }

        /// <summary>
        /// 解指定类型TCP包
        /// </summary>
        public static void UnpackTCPMessage<T>(Socket socket, out SenderType sender, out MessageType type, out T message)
        {
            UnpackTCPMessage(socket, out sender, out type, out byte[] data);
            message = Destroy.Serializer.NetDeserialize<T>(data);
        }

        /// <summary>
        /// 打UDP包
        /// </summary>
        public static byte[] PackUDPMessage<T>(SenderType sender, MessageType type, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes((ushort)sender);
            byte[] typeData = BitConverter.GetBytes((ushort)type);
            byte[] data = Destroy.Serializer.NetSerialize<T>(message); //使用Protobuf-net

            //packet
            datas.AddRange(senderData); // 2bytes
            datas.AddRange(typeData);   // 2bytes
            datas.AddRange(data);       // nbytes

            return datas.ToArray();
        }

        /// <summary>
        /// 解包指定类型UDP包
        /// </summary>
        public static void UnpackUDPMessage<T>(byte[] data, out SenderType sender, out MessageType type, out T message)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);
                sender = (SenderType)reader.ReadUInt16();           // 2bytes
                type = (MessageType)reader.ReadUInt16();            // 2bytes
                byte[] msgData = reader.ReadBytes(data.Length - 4); // nbytes

                message = Destroy.Serializer.NetDeserialize<T>(msgData);  //使用Protobuf-net
            }
        }
    }
}