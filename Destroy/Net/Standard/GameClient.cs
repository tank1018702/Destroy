namespace Destroy.Net
{
    using System;
    using System.Net.Sockets;

    public class Client : NetClient
    {
        public Client(string serverIp, int serverPort) : base(serverIp, serverPort)
        {
        }

        protected override void OnConnected(Socket client)
        {
            base.OnConnected(client);
            Console.WriteLine("成功连接");
        }

        protected override void OnDisConnected(Socket client)
        {
            base.OnDisConnected(client);
            Console.WriteLine("断开连接");
        }
    }
}