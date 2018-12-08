namespace Destroy
{
    using System;
    using System.Net.Sockets;
    using System.Collections.Generic;
    using ProtoBuf;

    public static class NetworkSystem
    {
        private static Dictionary<int, GameObject> netPrefabs;
        private static List<NetworkTransform> networkTransforms;

        public static void Init(Dictionary<int, GameObject> netPrefabs)
        {
            NetworkSystem.netPrefabs = netPrefabs;
            networkTransforms = new List<NetworkTransform>();
            //foreach (GameObject gameObject in gameObjects)
            //{
            //    if (!gameObject.Active)
            //        continue;
            //    NetworkTransform networkTransform = gameObject.GetComponent<NetworkTransform>();
            //    if (!networkTransform || !networkTransform.Active)
            //        continue;

            //    if (!networkTransforms.Contains(networkTransform))
            //        networkTransforms.Add(networkTransform);
            //}
        }

        private static bool choose = false;
        private static Server server;
        private static Client client;
        private static float serverTimer;
        private static float clientTimer;

        internal static void Update(List<GameObject> gameObjects)
        {
            return;
            if (!choose)
            {
                choose = true;
                Print.DrawLine("1.client, 2.server", ConsoleColor.White);
                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
                        client = new Client(NetworkUtils.LocalIPv4Str, 8848);
                        client.Start();
                        break;
                    case 2:
                        server = new Server(8848);
                        server.Start();
                        break;
                    default:
                        throw new Exception();
                }
                Console.Clear();
            }
            if (server != null)
            {
                serverTimer += Time.DeltaTime;
                server.Update();         //服务器每帧刷新
                if (serverTimer >= 0.1f) //服务器1秒广播10次
                {
                    server.Broadcast();
                    serverTimer = 0;
                }
            }
            if (client != null)
            {
                clientTimer += Time.DeltaTime;
                client.Update();          //客户端每帧刷新
                if (clientTimer >= 0.05f) //客户端1秒同步20次
                {
                    client.Sync(1, 1);
                    clientTimer = 0;
                }
            }
        }
    }

    public class Server : NetworkServer
    {
        private int id;
        private int frame;
        Dictionary<Socket, Position> positions;

        public Server(int port) : base(port)
        {
            id = 0;
            frame = 0;
            positions = new Dictionary<Socket, Position>();
            base.OnConnected += OnConnected;
            base.OnDisconnected += OnDisconnected;
            Register((ushort)Role.Client, (ushort)Command.PosSync, PosSync);
        }

        private new void OnConnected(Socket socket)
        {
            Console.WriteLine("客户端连接");

            //初始化
            Position position = new Position(id, 0, 0);
            id++;
            positions.Add(socket, position);

            S2C_StartSync startSync = new S2C_StartSync();
            startSync.Frame = frame;
            startSync.YourId = position.Id;
            startSync.Positions = new List<Position>();
            foreach (var each in positions.Values)
                startSync.Positions.Add(each);

            //初始化
            Send(socket, (ushort)Role.Server, (ushort)Command.StartSync, startSync);
        }

        private new void OnDisconnected(string msg, Socket socket)
        {
            Console.WriteLine(msg);
            positions.Remove(socket);
        }

        private void PosSync(Socket socket, byte[] data)
        {
            C2S_PosSync posSync = Serializer.NetDeserialize<C2S_PosSync>(data);
            positions[socket] = posSync.Position; //修改数据

            Console.WriteLine(positions[socket].Id + " " + positions[socket].X);
        }

        public void Broadcast()
        {
            bool send = false;
            foreach (var each in positions)
            {
                S2C_PosSync posSync = new S2C_PosSync();
                posSync.Frame = frame;
                posSync.Positions = new List<Position>();
                foreach (var pos in positions.Values)
                    posSync.Positions.Add(pos);

                Send(each.Key, (ushort)Role.Server, (ushort)Command.PosSync, posSync);
                send = true;
            }
            if (send)
                Console.WriteLine("广播");
        }
    }

    public class Client : NetworkClient
    {
        private bool start;
        private int id;
        private int frame;
        private List<Position> positions;

        public Client(string serverIp, int serverPort) : base(serverIp, serverPort)
        {
            start = false;
            id = -1;
            frame = -1;
            positions = new List<Position>();
            OnConnected += socket => { Console.WriteLine("连接成功"); };
            OnDisConnected += (msg, socket) => { Console.WriteLine("掉线" + msg); };
            Register((ushort)Role.Server, (ushort)Command.StartSync, StartSync);
            Register((ushort)Role.Server, (ushort)Command.PosSync, PosSync);
        }

        private void StartSync(byte[] data)
        {
            S2C_StartSync startSync = Serializer.NetDeserialize<S2C_StartSync>(data);
            id = startSync.YourId;
            frame = startSync.Frame;
            positions = startSync.Positions;
            start = true;
        }

        private void PosSync(byte[] data)
        {
            Console.WriteLine("客户端posSync回调");

            S2C_PosSync posSync = Serializer.NetDeserialize<S2C_PosSync>(data);
            //更新位置
            frame = posSync.Frame;
            positions = posSync.Positions;

            foreach (var pos in positions)
            {
                Console.WriteLine(pos.X + " " + pos.Y);
            }
        }

        public void Sync(int x, int y)
        {
            if (!start)
                return;
            C2S_PosSync posSync = new C2S_PosSync();
            posSync.Frame = frame;
            posSync.Position = new Position(id, x, y);
            //同步自己坐标
            Send((ushort)Role.Client, (ushort)Command.PosSync, posSync);
        }
    }




    public enum Role
    {
        Client,
        Server
    }

    public enum Command
    {
        StartSync,
        PosSync,
    }

    [ProtoContract]
    public struct Position
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public int X;
        [ProtoMember(3)]
        public int Y;

        public Position(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }

    [ProtoContract]
    public struct C2S_PosSync
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public Position Position;
    }

    [ProtoContract]
    public struct S2C_PosSync
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public List<Position> Positions;
    }

    [ProtoContract]
    public struct S2C_StartSync
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int YourId;
        [ProtoMember(3)]
        public List<Position> Positions;
    }
}