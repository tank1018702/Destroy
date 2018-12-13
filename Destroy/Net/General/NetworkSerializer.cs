namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;

#if Protobuf
    using Google.Protobuf;

    public static partial class NetworkSerializer
    {
        public static byte[] ProtoSerializer<T>(T obj) where T : Google.Protobuf.IMessage
        {
            byte[] data = obj.ToByteArray();
            return data;
        }

        public static T ProtoDeserializer<T>(byte[] data) where T : Google.Protobuf.IMessage, new()
        {
            IMessage message = new T();
            T msg = (T)message.Descriptor.Parser.ParseFrom(data);
            return msg;
        }
    }
#endif

    public static partial class NetworkSerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        public static byte[] Serialize<T>(T t)
        {
            byte[] data = null;

            using (Stream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, t);
                data = new byte[stream.Length];

                MemoryStream memoryStream = new MemoryStream(data);
                ProtoBuf.Serializer.Serialize(memoryStream, t);
                BinaryReader reader = new BinaryReader(memoryStream);
                reader.Read(data, 0, data.Length);
            }
            return data;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static T Deserialize<T>(byte[] data) where T : new()
        {
            using (Stream stream = new MemoryStream(data))
            {
                T t = ProtoBuf.Serializer.Deserialize<T>(stream); //反序列化时必须保证类型拥有无参构造
                return t;
            }
        }

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
            byte[] data = NetworkSerializer.Serialize(message);
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
            byte[] data = NetworkSerializer.Serialize(message); //使用Protobuf-net

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