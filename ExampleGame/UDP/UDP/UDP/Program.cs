using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destroy.Net;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDP
{
    class Program
    {
        public static void Main()
        {
            UDPRoom room = new UDPRoom(9999);

            byte[] data = Encoding.UTF8.GetBytes("Hello World");
            var target = new IPEndPoint(NetworkUtils.LocalIPv4, 6666);

            while (true)
            {
                //room.Send(data, target);
                //room.Ping(target);
                //room.BroadCast(data, 6666);
                //long time = UDPRoom.Ping(target, 5000);
                long time = room.Ping(target, 5000);
                Console.WriteLine(time);
                Thread.Sleep(100);
            }
        }
    }
}