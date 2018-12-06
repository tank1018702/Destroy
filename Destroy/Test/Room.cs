namespace Destroy
{
    using System.Collections.Generic;

    public class Room
    {
        public enum GameState
        {
            Room,
            Game,
        }

        public readonly int RoomId;
        public readonly int MaxPlayerAmount;
        public readonly List<__Player> Players;
        public GameState State;

        public Room(int roomId, int maxPlayerAmount)
        {
            RoomId = roomId;
            MaxPlayerAmount = maxPlayerAmount;
            Players = new List<__Player>();
            State = GameState.Room;
        }
    }
}