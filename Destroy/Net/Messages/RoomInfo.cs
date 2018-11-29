namespace Destroy.Net
{
    using ProtoBuf;

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