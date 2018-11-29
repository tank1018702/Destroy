namespace Destroy.Net
{
    using ProtoBuf;

    [ProtoContract]
    public struct PlayerInfo
    {
        [ProtoMember(1)]
        public string Name;
    }
}