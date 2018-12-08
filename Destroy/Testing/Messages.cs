namespace Destroy.Testing
{
    using ProtoBuf;

    [ProtoContract]
    public struct PlayerInfo
    {
        [ProtoMember(1)]
        public string Name;
    }

    [ProtoContract]
    public struct Position
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [ProtoContract]
    public struct RoomInfo
    {
        [ProtoMember(1)]
        public int CurAmount;
        [ProtoMember(2)]
        public int MaxAmount;

        public RoomInfo(int curAmount, int maxAmount)
        {
            CurAmount = curAmount;
            MaxAmount = maxAmount;
        }
    }
}