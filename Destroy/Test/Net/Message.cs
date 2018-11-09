namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public enum SenderType
    {
        Server,
        Client
    }

    public enum MessageType
    {

    }

    public static class Message
    {
        public static int EnumToKey(SenderType sender, MessageType type)
        {
            ushort temp = (ushort)((ushort)sender << 8);
            ushort key = (ushort)(temp + (ushort)type);
            return key;
        }

        /// <summary>
        /// 把发送者类型, 消息类型, 消息打成一个包
        /// </summary>
        public static byte[] PackTCPMessage<T>(SenderType sender, MessageType type, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes((ushort)sender);
            byte[] typeData = BitConverter.GetBytes((ushort)type);
            //使用Protobuf-net
            byte[] data = Serializer.NetSerialize(message);
            byte[] bodyLen = BitConverter.GetBytes
                ((ushort)(senderData.Length + typeData.Length + data.Length));

            //packet head
            datas.AddRange(bodyLen);     // 2bytes (the length of the packet body)
            //packet body
            datas.AddRange(senderData);  // 2bytes
            datas.AddRange(typeData);    // 2bytes
            datas.AddRange(data);        // nbytes

            return datas.ToArray();
        }

        /// <summary>
        /// 利用Socket读取一个打好的包
        /// </summary>
        public static void UnpackTCPMessage<T>(Socket socket, out SenderType sender, out MessageType type, out T message)
        {
            ushort bodyLen;
            byte[] head = new byte[2];
            socket.Receive(head);

            bodyLen = BitConverter.ToUInt16(head, 0);            // 2bytes (the length of the packet body)
            byte[] body = new byte[bodyLen];
            socket.Receive(body);

            using (MemoryStream stream = new MemoryStream(body))
            {
                BinaryReader reader = new BinaryReader(stream);
                sender = (SenderType)reader.ReadUInt16();        // 2bytes
                type = (MessageType)reader.ReadUInt16();         // 2bytes
                byte[] data = reader.ReadBytes(bodyLen - 4);     // nbytes
                //使用Protobuf-net
                message = Serializer.NetDeserialize<T>(data);
            }
        }

        public static byte[] PackUDPMessage<T>(SenderType sender, MessageType type, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes((ushort)sender);
            byte[] typeData = BitConverter.GetBytes((ushort)type);
            //使用Protobuf-net
            byte[] data = Serializer.NetSerialize<T>(message);

            //packet
            datas.AddRange(senderData); // 2bytes
            datas.AddRange(typeData);   // 2bytes
            datas.AddRange(data);       // nbytes

            return datas.ToArray();
        }

        public static void UnpackUDPMessage<T>(byte[] data, out SenderType sender, out MessageType type, out T message)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);
                sender = (SenderType)reader.ReadUInt16();  // 2bytes
                type = (MessageType)reader.ReadUInt16();
                byte[] msgData = reader.ReadBytes(data.Length - 4);
                //使用Protobuf-net
                message = Serializer.NetDeserialize<T>(msgData);
            }
        }
    }
}