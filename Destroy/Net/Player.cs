namespace Destroy
{
    public class Player
    {
        public bool InRoom => Room != null;

        public readonly int Id;
        public Room Room { get; private set; }
        public string Name;

        public Player(int id)
        {
            Id = id;
            Room = null;
            Name = null;
        }

        public void EnterRoom(Room room)
        {
            if (InRoom)
                return;
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
}