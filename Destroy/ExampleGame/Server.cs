//namespace Destroy.ExampleGame
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Net;
//    using System.Net.Sockets;
//    using System.Text;
//    using System.Threading;
//    using Destroy;
//    using Destroy.Graphics;

//    public class Player
//    {
//        public byte[] Data { get; private set; }
//        private Socket socket;
//        private IAsyncResult async;

//        public Player(Socket socket)
//        {
//            this.socket = socket;
//        }

//        public bool TryReceive(int length)
//        {
//            if (async != null && !async.IsCompleted) //正在收包
//                return false;
//            if (async != null && async.IsCompleted)  //收到包
//            {
//                socket.EndReceive(async);
//                async = null;
//                return true;
//            }

//            Data = new byte[length];
//            async = socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, null, null);

//            if (async.IsCompleted) //如果立马完成就直接返回
//            {
//                socket.EndReceive(async);
//                async = null;
//                return true;
//            }
//            else
//                return false;
//        }

//        public void Send(byte[] data)
//        {
//            socket.Send(data);
//        }
//    }

//    [CreatGameObject(1, "Server")]
//    public class Server : Script
//    {
//        private object locker = new object();

//        private List<Player> players;
//        private bool start;
//        private int frameIndex;
//        private int playerId;
//        private Dictionary<int, int[]> frame_cmds;

//        public override void Start()
//        {
//            players = new List<Player>();
//            start = false;
//            frameIndex = 0;
//            playerId = 1;
//            frame_cmds = new Dictionary<int, int[]>();
//            //string[,] items =
//            //{
//            //    {"┌", "─", "┐"},
//            //    {"│", " ", "│"},
//            //    {"│", " ", "│"},
//            //    {"└", "─", "┘"}
//            //};
//            //Block block = new Block(items, 2, CoordinateType.Window);
//            //RendererSystem.RenderBlock(block);
//            Console.Title = "Game";
//            Window window = new Window(40, 20);
//            window.SetIOEncoding(Encoding.Unicode);

//            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
//            serverSocket.Bind(serverEP);
//            serverSocket.Listen(0);

//            Thread accept = new Thread(Accept) { IsBackground = true };
//            Thread receive = new Thread(Receive) { IsBackground = true };
//            Thread send = new Thread(Send) { IsBackground = true };

//            accept.Start(serverSocket);
//            receive.Start();
//            send.Start();

//            Console.WriteLine("服务器开启成功!");
//        }

//        void Accept(object param)
//        {
//            Socket serverSocket = (Socket)param;

//            while (true)
//            {
//                Socket clientSocket = serverSocket.Accept();
//                Player player = new Player(clientSocket);

//                //发送登陆id
//                ServerLoginMsg serverLoginMsg = new ServerLoginMsg();
//                serverLoginMsg.id = playerId;
//                playerId++;
//                byte[] data = PorscheHelper.Serialize2Bit(serverLoginMsg);
//                player.Send(data);

//                lock (locker)
//                {
//                    players.Add(player); //Write
//                }

//                if (players.Count == 2)
//                {
//                    Console.WriteLine("开始游戏!");
//                    start = true;
//                    Broadcast();
//                }
//            }
//        }

//        void Receive()
//        {
//            while (true)
//            {
//                foreach (var player in players)
//                {
//                    bool received = player.TryReceive(8);
//                    if (!received)
//                        continue;

//                    ClientMoveCmd clientMove = PorscheHelper.Deserialize4Bit<ClientMoveCmd>(player.Data);
//                    if (clientMove.id == 1)
//                        player1MoveCmd = clientMove.moveCmd;
//                    else
//                        player2MoveCmd = clientMove.moveCmd;
//                }
//                Thread.Sleep(1);
//            }
//        }

//        void Send()
//        {
//            while (true)
//            {

//            }
//        }

//        void Broadcast()
//        {
//            foreach (var player in players)
//            {
//                ServerFrameMsg serverFrameMsg = new ServerFrameMsg();
//                serverFrameMsg.frameIndex = frameIndex;
//                serverFrameMsg.player1MoveCmd = frame_cmds[frameIndex][0];
//                serverFrameMsg.player2MoveCmd = frame_cmds[frameIndex][1];
//                byte[] data = PorscheHelper.Serialize2Bit(serverFrameMsg);
//                player.Send(data);
//            }
//        }
//    }
//}