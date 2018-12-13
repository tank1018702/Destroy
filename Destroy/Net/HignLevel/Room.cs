namespace Destroy.Net
{
    using System.Collections.Generic;

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
}