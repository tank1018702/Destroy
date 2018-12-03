using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Destroy;

namespace DevTest
{
    public class Server : NetworkServer
    {
        public Server(int port) : base(port)
        {
        }

        protected override void OnConnected(Socket client)
        {
            Console.WriteLine("客户端连接");
        }

        protected override void OnDisconnected(Socket client)
        {
            Console.WriteLine("客户端掉线");
        }
    }

    [CreatGameObject]
    public class Bootstrap : Script
    {
        public override void Start()
        {
            Server server = new Server(8848);
            NetworkSystem.Init(server, null);
        }
    }

    class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(50);
        }
    }
}