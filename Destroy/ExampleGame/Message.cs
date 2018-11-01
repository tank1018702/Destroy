namespace Destroy.ExampleGame
{
    public enum MoveCmd
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
    }

    public class ClientMoveCmd
    {
        public int id;
        public int moveCmd;
    }

    public class ServerLoginMsg
    {
        public int id;
    }

    public class ServerFrameMsg
    {
        public int frameIndex;
        public int player1MoveCmd;
        public int player2MoveCmd;
    }
}