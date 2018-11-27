namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;

    public delegate void MessageEvent(object obj, byte[] data);

    public static class Message
    {
        public static int EnumToKey(ushort cmd1, ushort cmd2)
        {
            ushort temp = (ushort)(cmd1 << 8);
            ushort key = (ushort)(temp + cmd2);
            return key;
        }

        /// <summary>
        /// 打TCP包
        /// </summary>
        public static byte[] PackTCPMessage<T>(ushort cmd1, ushort cmd2, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes(cmd1);
            byte[] typeData = BitConverter.GetBytes(cmd2);
            //使用Protobuf-net
            byte[] data = Serializer.NetSerialize(message);
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
        public static void UnpackTCPMessage(Socket socket, out ushort cmd1, out ushort cmd2, out byte[] data)
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
                cmd1 = reader.ReadUInt16();                     // 2bytes
                cmd2 = reader.ReadUInt16();                     // 2bytes
                data = reader.ReadBytes(bodyLen - 4);           // nbytes
            }
        }

        /// <summary>
        /// 解指定类型TCP包
        /// </summary>
        public static void UnpackTCPMessage<T>(Socket socket, out ushort cmd1, out ushort cmd2, out T message)
        {
            UnpackTCPMessage(socket, out cmd1, out cmd2, out byte[] data);
            message = Serializer.NetDeserialize<T>(data);
        }

        /// <summary>
        /// 打UDP包
        /// </summary>
        public static byte[] PackUDPMessage<T>(ushort cmd1, ushort cmd2, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] senderData = BitConverter.GetBytes(cmd1);
            byte[] typeData = BitConverter.GetBytes(cmd2);
            byte[] data = Serializer.NetSerialize(message); //使用Protobuf-net

            //packet
            datas.AddRange(senderData); // 2bytes
            datas.AddRange(typeData);   // 2bytes
            datas.AddRange(data);       // nbytes

            return datas.ToArray();
        }

        /// <summary>
        /// 解包指定类型UDP包
        /// </summary>
        public static void UnpackUDPMessage<T>(byte[] data, out ushort cmd1, out ushort cmd2, out T message)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);
                cmd1 = reader.ReadUInt16();                         // 2bytes
                cmd2 = reader.ReadUInt16();                         // 2bytes
                byte[] msgData = reader.ReadBytes(data.Length - 4); // nbytes

                message = Serializer.NetDeserialize<T>(msgData);    //使用Protobuf-net
            }
        }
    }
}