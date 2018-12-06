namespace Destroy.Test
{
    using ProtoBuf;

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
}