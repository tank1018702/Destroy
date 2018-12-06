namespace Destroy
{
    /// <summary>
    /// TODO
    /// </summary>
    public class RoomServer
    {
        private int playerId;
        private Room room;
        private UDPService server;

        public RoomServer(int roomId, int maxPlayerAmount)
        {
            playerId = 0;
            room = new Room(roomId, maxPlayerAmount);
        }
    }
}