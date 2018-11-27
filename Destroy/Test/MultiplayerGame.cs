//namespace Destroy.Test
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Net.Sockets;
//    using System.Threading;
//    using Destroy;
//    using Destroy.Net;
//    using ProtoBuf;

//    public enum GameState
//    {
//        Waiting,
//        Playing,
//    }

//    public enum Room
//    {
//        S2C_StartSync
//    }

//    //Request  (Client to Server)
//    //Response (Server to Client)
//    public enum Game
//    {
//        C2S_PosSync,
//        S2C_PosSync,
//    }

//    [ProtoContract]
//    public class Player
//    {
//        [ProtoMember(1)]
//        public int Id;
//        [ProtoMember(2)]
//        public int X;
//        [ProtoMember(3)]
//        public int Y;
//    }

//    [ProtoContract]
//    public class C2S_PosSync
//    {
//        [ProtoMember(1)]
//        public int Frame;
//        [ProtoMember(2)]
//        public Player Self;
//    }

//    [ProtoContract]
//    public class S2C_StartSync
//    {
//        [ProtoMember(1)]
//        public int Frame;
//        [ProtoMember(2)]
//        public int SelfId;
//        [ProtoMember(3)]
//        public List<Player> Players;
//    }

//    [ProtoContract]
//    public class S2C_PosSync
//    {
//        [ProtoMember(1)]
//        public int Frame;
//        [ProtoMember(2)]
//        public List<Player> Players;
//    }

//    [CreatGameObject(0)]
//    public class MultiplayerGame : Script
//    {
//        Client client;

//        public override void Start()
//        {
//            Server server = new Server(8848);
//            server.Start();
//            client = new Client(NetworkUtils.LocalIPv4Str, 8848);
//            client.Connect();
//        }

//        public override void Update()
//        {
//            if (Input.GetKey(KeyCode.A))
//                transform.Translate(Vector2Int.Left);
//            if (Input.GetKey(KeyCode.D))
//                transform.Translate(Vector2Int.Right);
//            if (Input.GetKey(KeyCode.W))
//                transform.Translate(Vector2Int.Up);
//            if (Input.GetKey(KeyCode.S))
//                transform.Translate(Vector2Int.Down);
//            //更新坐标
//            client.UpdatePos(transform.Position);
//        }
//    }

//    public class Client : NetworkClient
//    {
//        private int frame;
//        private int id;
//        private List<Player> players;

//        public Client(string serverIp, int serverPort) : base(serverIp, serverPort)
//        {
//            frame = 0;
//            id = 0;
//            players = new List<Player>();
//        }

//        private void OnStartGame(object obj, byte[] data)
//        {
//        }

//        public void UpdatePos(Vector2Int position)
//        {
//        }
//    }

//    public class Server : NetworkServer
//    {
//        private int frameIndex;
//        private int playerId;
//        private List<Player> players;

//        public Server(int port) : base(port)
//        {
//            frameIndex = 0;
//            playerId = 0;
//            players = new List<Player>();
//        }

//        protected override object OnAccept(TcpClient tcpClient)
//        {
//            //初始坐标为0
//            Player player = new Player(playerId++, tcpClient);
//            players.Add(player);
//            //达到两人开始游戏
//            if (players.Count == 1)
//            {
//                Register((ushort)Cmd1.None, (ushort)Cmd2.Move, OnPlayerMove);
//                //广播开始游戏
//                foreach (var each in players)
//                {
//                    StartGame startGame = new StartGame();
//                    startGame.Id = player.Id;
//                    startGame.Players = new List<int>();
//                    foreach (var item in players)
//                        startGame.Players.Add(item.Id);

//                    Send(each.Client, (ushort)Cmd1.None, (ushort)Cmd2.StartGame, startGame);
//                }
//                //开始状态同步
//                Broadcast();
//            }
//            return player;
//        }

//        private void Broadcast()
//        {
//            Thread broadcast = new Thread(_Broadcast) { IsBackground = true };
//            broadcast.Start();

//            void _Broadcast()
//            {
//                while (true)
//                {
//                    foreach (var each in players)
//                    {
//                        PlayerMove playerMove = new PlayerMove();
//                        playerMove.X = each.X;
//                        playerMove.Y = each.Y;
//                        Send(each.Client, (ushort)Cmd1.None, (ushort)Cmd2.Move, playerMove);
//                    }
//                    frameIndex++;
//                    Thread.Sleep(50);
//                }
//            }
//        }

//        private void OnPlayerMove(object obj, byte[] data)
//        {
//            Player player = (Player)obj;
//            PlayerMove playerMove = Destroy.Serializer.NetDeserialize<PlayerMove>(data);
//            lock (this)
//            {
//                player.X = playerMove.X;
//                player.Y = playerMove.Y;
//            }
//        }
//    }
//}