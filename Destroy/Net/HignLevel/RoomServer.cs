using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy.Net
{
    public class RoomServer
    {
        private Room room;
        private UDPService server;

        public RoomServer(string ip, int port, int roomId, int maxPlayerAmount)
        {
            server = new UDPService(ip, port);
            room = new Room(roomId, maxPlayerAmount);
        }

        public void Open(int targetPort)
        {
            S2C_RoomInfo roomInfo = new S2C_RoomInfo();
            roomInfo.RoomId = room.RoomId;
            roomInfo.MaxPlayerAmount = room.MaxPlayerAmount;
            roomInfo.CurPlayerAmount = room.Players.Count;

            server.Broadcast(targetPort, (ushort)NetworkRole.Server, (ushort)RoomCmd.Broadcast, roomInfo);
        }

        public void Close()
        {

        }
    }
}