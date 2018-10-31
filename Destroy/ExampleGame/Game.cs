namespace Destroy.ExampleGame
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Destroy;
    using Destroy.Graphics;

    [CreatGameObject(1, "Server")]
    public class Server : Script
    {
        private ConcurrentQueue<byte[]> messages;

        public override void Start()
        {
            //string[,] items =
            //{
            //    {"┌", "─", "┐"},
            //    {"│", " ", "│"},
            //    {"│", " ", "│"},
            //    {"└", "─", "┘"}
            //};
            //Block block = new Block(items, 2, CoordinateType.RightX_DownY);
            //RendererSystem.RenderBlock(block);
            Console.Title = "Game";
            Window window = new Window(40, 20);
            window.SetIOEncoding(Encoding.Unicode);

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
            serverSocket.Bind(serverEP);
            serverSocket.Listen(0);

            Thread accept = new Thread(Accept) { IsBackground = true };
            Thread receive = new Thread(Receive) { IsBackground = true };
            Thread send = new Thread(Send) { IsBackground = true };

            accept.Start(serverSocket);
            receive.Start();
            send.Start();

            Console.WriteLine("服务器开启成功!");
        }

        void Accept(object param)
        {
            Socket serverSocket = (Socket)param;

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

            }
        }

        void Receive()
        {
            while (true)
            {

            }
        }

        void Send()
        {
            while (true)
            {

            }
        }
    }

    [CreatGameObject(2, "Client")]
    public class Client : Script
    {
        public override void Start()
        {
            Thread receive = new Thread(Receive) { IsBackground = true };
            Thread send = new Thread(Send) { IsBackground = true };
            receive.Start();
            send.Start();
        }

        public override void Update(float deltaTime)
        {

        }

        void Receive()
        {
            while (true)
            {

            }
        }

        void Send()
        {
            while (true)
            {

            }
        }
    }
}