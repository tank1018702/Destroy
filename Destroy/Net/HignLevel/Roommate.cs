namespace Destroy.Net
{
    public class Roommate
    {
        public string Name { get; set; }

        public bool InRoom => Room != null;

        public int Id { get; private set; }

        public Room Room { get; private set; }

        public Roommate(int id)
        {
            Name = null;
            Id = id;
            Room = null;
        }

        public void EnterRoom(Room room)
        {
            if (InRoom)
                ExitRoom();
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