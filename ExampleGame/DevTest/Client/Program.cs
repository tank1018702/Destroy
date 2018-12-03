using System;
using System.Net.Sockets;
using Destroy;

namespace Client
{
    public class Client : NetworkClient
    {
        public Client(string serverIp, int serverPort) : base(serverIp, serverPort)
        {
        }

        protected override void OnConnected(Socket client)
        {
            Console.WriteLine("连接成功");
        }

        protected override void OnDisConnected(Socket client)
        {
            Console.WriteLine("断开连接");
        }
    }

    [CreatGameObject]
    public class Bootstrap : Script
    {
        public override void Start()
        {
            Client client = new Client(NetworkUtils.LocalIPv4Str, 8848);
            NetworkSystem.Init(null, client);
        }
    }

    public class Program
    {
        static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(50);
        }
    }
}