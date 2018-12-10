namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;

    public static class NetworkMessage
    {
        /// <summary>
        /// 枚举转整数
        /// </summary>
        public static int EnumToKey(ushort cmd1, ushort cmd2)
        {
            ushort temp = (ushort)(cmd1 << 8);
            ushort key = (ushort)(temp + cmd2);
            return key;
        }

        /// <summary>
        /// 打简单TCP包
        /// </summary>
        public static byte[] PackSimpleTCPMessage(ushort cmd1, ushort cmd2, byte[] data)
        {
            List<byte> datas = new List<byte>();

            byte[] data1 = BitConverter.GetBytes(cmd1);
            byte[] data2 = BitConverter.GetBytes(cmd2);
            byte[] bodyLen = BitConverter.GetBytes((ushort)(data1.Length + data2.Length + data.Length));

            //packet head
            datas.AddRange(bodyLen); // 2bytes (the length of the packet body)
            //packet body
            datas.AddRange(data1);   // 2bytes
            datas.AddRange(data2);   // 2bytes
            datas.AddRange(data);    // nbytes

            return datas.ToArray();
        }

        /// <summary>
        /// 打TCP包
        /// </summary>
        public static byte[] PackTCPMessage<T>(ushort cmd1, ushort cmd2, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] data1 = BitConverter.GetBytes(cmd1);
            byte[] data2 = BitConverter.GetBytes(cmd2);
            //使用Protobuf-net
            byte[] data = Serializer.NetSerialize(message);
            byte[] bodyLen = BitConverter.GetBytes((ushort)(data1.Length + data2.Length + data.Length));

            //packet head
            datas.AddRange(bodyLen);     // 2bytes (the length of the packet body)
            //packet body
            datas.AddRange(data1);       // 2bytes
            datas.AddRange(data2);       // 2bytes
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

            bodyLen = BitConverter.ToUInt16(head, 0);     // 2bytes (the length of the packet body)
            byte[] body = new byte[bodyLen];
            socket.Receive(body);

            using (MemoryStream memory = new MemoryStream(body))
            {
                using (BinaryReader reader = new BinaryReader(memory))
                {
                    cmd1 = reader.ReadUInt16();           // 2bytes
                    cmd2 = reader.ReadUInt16();           // 2bytes
                    data = reader.ReadBytes(bodyLen - 4); // nbytes
                }
            }
        }

        /// <summary>
        /// 打UDP包
        /// </summary>
        public static byte[] PackUDPMessage<T>(ushort cmd1, ushort cmd2, T message)
        {
            List<byte> datas = new List<byte>();

            byte[] data1 = BitConverter.GetBytes(cmd1);
            byte[] data2 = BitConverter.GetBytes(cmd2);
            byte[] data = Serializer.NetSerialize(message); //使用Protobuf-net

            //packet
            datas.AddRange(data1);      // 2bytes
            datas.AddRange(data2);      // 2bytes
            datas.AddRange(data);       // nbytes

            return datas.ToArray();
        }

        /// <summary>
        /// 解UDP包
        /// </summary>
        public static void UnpackUDPMessage(byte[] data, out ushort cmd1, out ushort cmd2, out byte[] msgData)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    cmd1 = reader.ReadUInt16();                         // 2bytes
                    cmd2 = reader.ReadUInt16();                         // 2bytes
                    msgData = reader.ReadBytes(data.Length - 4);        // nbytes
                }
            }
        }
    }
}