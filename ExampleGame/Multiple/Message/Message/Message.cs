namespace Boxhead.Message
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using Destroy;
    using Destroy.Net;
    using ProtoBuf;

    public enum ActionType : ushort
    {
        Client = 0,
        Server = 1,
    }

    public enum MessageType : ushort
    {
        None = 0,
        //Server
        FrameSync = 1,
        StartGame = 2,
        //Client
        PlayerInput = 3,
    }

    [ProtoContract]
    public class PlayerInput : IMessage
    {
        [ProtoMember(1)]
        public int frameIndex;          // 4bytes
        [ProtoMember(2)]
        public bool up;                 // 1byte
        [ProtoMember(3)]
        public bool down;               // 1byte
        [ProtoMember(4)]
        public bool left;               // 1byte
        [ProtoMember(5)]
        public bool right;              // 1byte
    }

    [ProtoContract]
    public class FrameSync : IMessage
    {
        [ProtoMember(1)]
        public int frameIndex;                  // 4bytes server current frame
        [ProtoMember(2)]
        public ConcurrentDictionary<int, ConcurrentDictionary<int, PlayerInput>> playerInputs;
    }

    [ProtoContract]
    public class StartGame : IMessage
    {
        [ProtoMember(1)]
        public int playerId;                   // 4bytes
        [ProtoMember(2)]
        public List<int> players;
    }

    public delegate void MessageEvent(object obj, byte[] data);

    public static class MessageSerializer
    {
        public static int Enum2Int(ActionType action, MessageType type)
        {
            ushort temp = (ushort)((ushort)action << 8);
            ushort key = (ushort)(temp + (ushort)type);
            return key;
        }

        public static byte[] SerializeMsg(ActionType action, MessageType type, IMessage message)
        {
            List<byte> list = new List<byte>();
            byte[] actionData = BitConverter.GetBytes((ushort)action);
            byte[] typeData = BitConverter.GetBytes((ushort)type);
            byte[] data = NetworkUtils.NetSerialize(message);
            byte[] bodyLen = BitConverter.GetBytes
                ((ushort)(actionData.Length + typeData.Length + data.Length));
            //packet head
            list.AddRange(bodyLen);     // 2bytes (the length of the packet body)
            //packet body
            list.AddRange(actionData);  // 2bytes
            list.AddRange(typeData);    // 2bytes
            list.AddRange(data);        // nbytes
            return list.ToArray();
        }

        public static byte[] DeserializeMsg(Socket socket, out ActionType action, out MessageType type)
        {
            ushort bodyLen;
            byte[] head = new byte[2];
            socket.Receive(head);
            bodyLen = BitConverter.ToUInt16(head, 0);
            byte[] body = new byte[bodyLen];
            socket.Receive(body);
            using (MemoryStream stream = new MemoryStream(body))
            {
                BinaryReader reader = new BinaryReader(stream);
                action = (ActionType)reader.ReadUInt16();
                type = (MessageType)reader.ReadUInt16();
                return reader.ReadBytes(bodyLen - 4);
            }
        }
    }
}