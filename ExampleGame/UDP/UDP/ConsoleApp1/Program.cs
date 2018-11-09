using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destroy.Net;
using System.Net;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPRoom room = new UDPRoom(6666);

            while (true)
            {
                room.ReceivePing();
                //byte[] data = room.Receive(out IPEndPoint remoteEP);
                //room.Send(data, remoteEP);

                //long time = UDPRoom.ReceivePing(target);
                //Console.WriteLine(time);
                //Thread.Sleep(1000);
            }
        }
    }
}
