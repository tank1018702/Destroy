namespace Destroy
{
    using System.Collections.Generic;

    public class Room
    {
        public readonly int RoomId;
        public readonly int MaxPlayerAmount;
        public readonly List<Player> Players;
        public GameState State;

        public Room(int roomId, int maxPlayerAmount)
        {
            RoomId = roomId;
            MaxPlayerAmount = maxPlayerAmount;
            Players = new List<Player>();
            State = GameState.Room;
        }
    }
}