namespace Destroy.Testing
{
    using ProtoBuf;
    using System.Collections.Generic;

    public class Roommate
    {
        public string Name { get; set; }

        public bool InRoom => Room != null;

        public int Id { get; private set; }

        public Room Room { get; private set; }

        public Roommate(int id)
        {
            Name = null;
            Id = id;
            Room = null;
        }

        public void EnterRoom(Room room)
        {
            if (InRoom)
                ExitRoom();
            room.Players.Add(this);
            Room = room;
        }

        public void ExitRoom()
        {
            if (!InRoom)
                return;
            Room.Players.Remove(this);
            Room = null;
        }
    }

    public class Room
    {
        public enum State
        {
            Room,
            Game,
        }

        public int RoomId { get; private set; }
        public int MaxPlayerAmount { get; private set; }
        public List<Roommate> Players { get; private set; }
        public State CurrentState { get; set; }

        public Room(int roomId, int maxPlayerAmount)
        {
            RoomId = roomId;
            MaxPlayerAmount = maxPlayerAmount;
            Players = new List<Roommate>();
            CurrentState = State.Room;
        }
    }

    public class RoomServer
    {
        private Room room;
        private UDPService server;

        public RoomServer(string ip, int port, int roomId, int maxPlayerAmount)
        {
            server = new UDPService(ip, port);
            room = new Room(roomId, maxPlayerAmount);
        }

        public void Open(int targetPort)
        {
            S2C_RoomInfo roomInfo = new S2C_RoomInfo();
            roomInfo.RoomId = room.RoomId;
            roomInfo.MaxPlayerAmount = room.MaxPlayerAmount;
            roomInfo.CurPlayerAmount = room.Players.Count;

            server.Broadcast(targetPort, (ushort)RoomRole.Server, (ushort)RoomCmd.Broadcast, roomInfo);
        }

        public void Close()
        {

        }
    }

    public class RoomClient
    {

    }

    public enum RoomRole
    {
        Server,
        Client,
    }

    public enum RoomCmd
    {
        Broadcast
    }

    [ProtoContract]
    public struct S2C_RoomInfo
    {
        [ProtoMember(1)]
        public int RoomId;
        [ProtoMember(2)]
        public int MaxPlayerAmount;
        [ProtoMember(3)]
        public int CurPlayerAmount;
    }

    [ProtoContract]
    public struct C2S_JoinRoom
    {
        [ProtoMember(1)]
        public string Name;
    }

    [ProtoContract]
    public struct S2C_JoinRoom
    {
        [ProtoMember(1)]
        public string IP;
    }
}