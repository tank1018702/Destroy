//namespace Destroy.Test
//{
//    using System;
//    using System.Collections.Concurrent;
//    using System.Collections.Generic;
//    using System.Net.Sockets;
//    using System.Threading;
//    using Destroy;
//    using ProtoBuf;

//    public enum GameState
//    {
//        Room,
//        Game,
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

//        public Player()
//        {
//        }

//        public Player(int id, int x, int y)
//        {
//            Id = id;
//            X = x;
//            Y = y;
//        }
//    }

//    [ProtoContract]
//    public class C2S_PosSync
//    {
//        [ProtoMember(1)]
//        public int Frame;
//        [ProtoMember(2)]
//        public Player Player;
//    }

//    [ProtoContract]
//    public class S2C_StartSync
//    {
//        [ProtoMember(1)]
//        public int Frame;
//        [ProtoMember(2)]
//        public int Id;
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

//#if !Client
//    [CreatGameObject]
//    public class MultiplayerServer : Script
//    {
//        private Server server;

//        public override void Start()
//        {
//            server = new Server(8848);
//            server.Start();
//            Console.WriteLine("服务器开启成功");
//        }

//        public override void Update()
//        {

//        }
//    }

//    public class Server : NetworkServer
//    {
//        private int frame;
//        private int playerId;
//        private Dictionary<TcpClient, Player> players;

//        public Server(int port) : base(port)
//        {
//            frame = 0;
//            playerId = 0;
//            players = new Dictionary<TcpClient, Player>();
//            Register((ushort)GameState.Game, (ushort)Game.C2S_PosSync, C2S_PosSync);
//        }

//        protected override void OnAccept(TcpClient tcpClient)
//        {
//            Player player = new Player(playerId, playerId, 0);
//            playerId++;
//            players.Add(tcpClient, player);

//            //达到两人开始游戏
//            if (players.Count == 2)
//            {
//                //广播开始游戏
//                foreach (var each in players)
//                {
//                    S2C_StartSync start = new S2C_StartSync();
//                    start.Frame = frame;
//                    start.Id = each.Value.Id;
//                    start.Players = new List<Player>(players.Values);


//                    Send(each.Key, (ushort)GameState.Room, (ushort)Room.S2C_StartSync, start);
//                }
//                //开始状态同步
//                Broadcast();
//            }
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
//                        S2C_PosSync posSync = new S2C_PosSync();
//                        posSync.Frame = frame;
//                        posSync.Players = new List<Player>(players.Values);

//                        Send(each.Key, (ushort)GameState.Game, (ushort)Game.S2C_PosSync, posSync);
//                    }
//                    lock (this) // Write frame
//                        frame++;

//                    Thread.Sleep(10); //一秒100Tick
//                }
//            }
//        }

//        private void C2S_PosSync(object obj, byte[] data)
//        {
//            var key = (TcpClient)obj;
//            C2S_PosSync pos = Destroy.Serializer.NetDeserialize<C2S_PosSync>(data);

//            lock (this)                    // Write players
//            {
//                if (pos.Frame + 4 < frame) //过时的包(客户端发包到服务器收包超过200ms)
//                    return;
//                Player player = players[key];
//                player.X = pos.Player.X;
//                player.Y = pos.Player.Y;
//            }
//        }
//    }
//#endif



//#if !Client
//    [CreatGameObject]
//    public class MultiplayerClient : Script
//    {
//        private int id;
//        private List<GameObject> gamers;
//        private Client client;

//        public static Action<List<Player>> Move;
//        public static Action<int, List<Player>> Creat;

//        void Spawn(Player player, bool self = false)
//        {
//            GameObject gamer = new GameObject();
//            gamer.transform.Position.X = player.X;
//            gamer.transform.Position.Y = player.Y;
//            Renderer renderer = gamer.AddComponent<Renderer>();
//            renderer.Order = 0;
//            renderer.Str = "吊";
//            renderer.ForeColor = ConsoleColor.Red;

//            gamers.Add(gamer);
//        }

//        public override void Start()
//        {
//            Input.RunInBackground = false;

//            gamers = new List<GameObject>();

//            GameObject camera = new GameObject();
//            Camera c = camera.AddComponent<Camera>();
//            c.CharWidth = 2;
//            RendererSystem.Init(camera);

//            Move += players =>
//            {
//                foreach (Player player in players)
//                {
//                    gamers[player.Id].transform.Position = new Vector2Int(player.X, player.Y);
//                }
//            };
//            Creat += (id, players) =>
//            {
//                this.id = id;
//                Player self = players[id];
//                Spawn(self, true);

//                foreach (Player player in players)
//                {
//                    if (player.Id == id)
//                        continue;
//                    Spawn(player);
//                }
//            };

//            Console.WriteLine("输入服务器IP Address");
//            string adr = Console.ReadLine();
//            Console.Clear();

//            client = new Client(adr, 8848);
//            client.Connect();
//        }

//        public override void Update()
//        {
//            if (gamers.Count == 0)
//                return;
//            Vector2Int pos = gamers[id].transform.Position;
//            if (Input.GetKey(KeyCode.A))
//                pos += Vector2Int.Left;
//            if (Input.GetKey(KeyCode.D))
//                pos += Vector2Int.Right;
//            if (Input.GetKey(KeyCode.W))
//                pos += Vector2Int.Up;
//            if (Input.GetKey(KeyCode.S))
//                pos += Vector2Int.Down;
//            //更新坐标
//            client.UpdatePos(pos);
//        }
//    }

//    public class Client : NetworkClient
//    {
//        private int frame;
//        private int id;
//        private List<Player> players;
//        private bool start;

//        public Client(string serverIp, int serverPort) : base(serverIp, serverPort)
//        {
//        }

//        protected override void OnConnect()
//        {
//            Register((ushort)GameState.Room, (ushort)Room.S2C_StartSync, S2C_StartSync);
//            Register((ushort)GameState.Game, (ushort)Game.S2C_PosSync, S2C_PosSync);
//        }

//        private void S2C_PosSync(byte[] data)
//        {
//            S2C_PosSync pos = Destroy.Serializer.NetDeserialize<S2C_PosSync>(data);
//            //Write
//            lock (this)
//            {
//                frame = pos.Frame;
//                players = pos.Players;
//                MultiplayerClient.Move(players);
//            }
//        }

//        private void S2C_StartSync(byte[] data)
//        {
//            S2C_StartSync start = Destroy.Serializer.NetDeserialize<S2C_StartSync>(data);
//            frame = start.Frame;
//            id = start.Id;
//            players = start.Players;
//            this.start = true;
//            MultiplayerClient.Creat(id, players);
//        }

//        public void UpdatePos(Vector2Int position)
//        {
//            if (!this.start)
//                return;
//            C2S_PosSync pos = new C2S_PosSync();
//            pos.Frame = frame;
//            pos.Player = new Player(id, position.X, position.Y);

//            Send((ushort)GameState.Game, (ushort)Game.C2S_PosSync, pos);
//        }
//    }
//#endif
//}