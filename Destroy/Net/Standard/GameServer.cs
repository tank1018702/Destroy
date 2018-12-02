namespace Destroy.Net
{
    using System;
    using System.Net.Sockets;

    public class Server : NetServer
    {
        public Server(int port) : base(port)
        {
        }
        
        protected override void OnConnected(Socket socket)
        {
            base.OnConnected(socket);
            Console.WriteLine("客户端连接");
        }

        protected override void OnDisconnected(Socket socket)
        {
            base.OnDisconnected(socket);
            Console.WriteLine("客户端断开连接");
        }
    }
}