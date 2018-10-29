namespace Destroy.Net
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class Room
    {
        public static void BroadCast(int targetPort, int rate)
        {
            //初始化自身IP与端口
            UdpClient sender = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            //设置目标的IP端口:IPAddress.Broadcast相当与255.255.255.255
            IPEndPoint target = new IPEndPoint(IPAddress.Broadcast, targetPort);
            //设置发送的消息
            byte[] data = Encoding.UTF8.GetBytes("enter room");

            //不断发送数据包
            while (true)
            {
                sender.Send(data, data.Length, target);
                Thread.Sleep(rate * 1000);
            }
        }

        public static void Receive(int port)
        {
            //初始化自身IP与端口
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            IPEndPoint iPEndPoint = null;

            while (true)
            {
                byte[] data = client.Receive(ref iPEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine(message);
            }
        }
    }
}