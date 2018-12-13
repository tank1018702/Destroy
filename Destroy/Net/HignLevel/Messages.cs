namespace Destroy.Net
{
    using ProtoBuf;
    using System.Collections.Generic;

    public enum NetworkRole
    {
        Client,
        Server,
    }

    #region Game

    public enum GameCmd
    {
        Join,
        Move,
        Instantiate,
        Destroy,
    }

    [ProtoContract]
    public struct C2S_Instantiate
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int TypeId;
        [ProtoMember(3)]
        public int X;
        [ProtoMember(4)]
        public int Y;
    }

    [ProtoContract]
    public struct S2C_Instantiate
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public Instance Instance;
    }

    [ProtoContract]
    public struct C2S_Destroy
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int TypeId;
        [ProtoMember(3)]
        public int Id;
    }

    [ProtoContract]
    public struct S2C_Destroy
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int TypeId;
        [ProtoMember(3)]
        public int Id;
    }

    [ProtoContract]
    public struct S2C_Join
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int YourId;
        [ProtoMember(3)]
        public List<Instance> Instances;
    }

    [ProtoContract]
    public struct Instance
    {
        [ProtoMember(1)]
        public int TypeId;
        [ProtoMember(2)]
        public int Id;
        [ProtoMember(3)]
        public bool IsLocal;
        [ProtoMember(4)]
        public int X;
        [ProtoMember(5)]
        public int Y;
    }

    [ProtoContract]
    public struct Entity
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public int X;
        [ProtoMember(3)]
        public int Y;

        public Entity(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }

    [ProtoContract]
    public struct C2S_Move
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public List<Entity> Entities; //Self Instances's Postions
    }

    [ProtoContract]
    public struct S2C_Move
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public List<Entity> Entities;
    }

    #endregion

    #region Room

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

    #endregion
}