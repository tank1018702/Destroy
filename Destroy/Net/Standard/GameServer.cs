namespace Destroy.Net
{
    using System;
    using System.Net.Sockets;

    public class Server : NetworkServer2
    {
        public Server(int port) : base(port)
        {
        }

        protected override void OnConnected(Socket socket)
        {
            Console.WriteLine("收到客户端连接");
        }

        protected override void OnDisconnected(Socket socket)
        {
        }
    }
}