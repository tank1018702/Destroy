namespace Destroy
{
    using System.Threading;
    using System.Net;

    public class RoomServer
    {
        private int playerId;
        private UDPRoom server;
        private Room room;

        private Thread broadcast;
        private Thread receive;

        public RoomServer(int roomId, int maxPlayerAmount)
        {
            playerId = 0;
            server = UDPRoom.CreatServer();
            room = new Room(roomId, maxPlayerAmount);
            broadcast = null;
            receive = null;
        }

        public void Open(int targetPort)
        {
            broadcast = new Thread(__Broadcast) { IsBackground = true };
            broadcast.Start(targetPort);

            receive = new Thread(__Receive) { IsBackground = true };
            receive.Start();
        }

        /// <summary>
        /// 一个线程
        /// </summary>
        private void __Broadcast(object param)
        {
            while (true)
            {
                RoomInfo roomInfo = new RoomInfo(room.Players.Count, room.MaxPlayerAmount);

                byte[] data = Serializer.NetSerialize(roomInfo);
                server.BroadCast(data, (int)param);
            }
        }

        /// <summary>
        /// 一个线程
        /// </summary>
        private void __Receive()
        {
            while (true)
            {
                byte[] data = server.Receive(out IPEndPoint remoteEP);

                PlayerInfo playerInfo = Serializer.NetDeserialize<PlayerInfo>(data);


                Player player = new Player(playerId);
                playerId++;
            }
        }
    }
}