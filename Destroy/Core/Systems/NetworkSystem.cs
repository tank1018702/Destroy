namespace Destroy
{
    using System;
    using Destroy.Standard;
    using ProtoBuf;
    using System.Collections.Generic;
    using System.Net.Sockets;

    public static class NetworkSystem
    {
        public static Instantiate CreatSelf;
        public static Instantiate CreatOther;

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
                //Print.DrawLine("1.client, 2.server", ConsoleColor.White);
                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
                        {
                            client = new Client(CreatSelf, CreatOther, NetworkUtils.LocalIPv4Str, 8848);
                        }
                        break;
                    case 2:
                        {
                            server = new Server(8848);
                        }
                        break;
                    default:
                        {
                            throw new Exception();
                        }
                }
                Console.Clear();
            }

            serverTimer += Time.DeltaTime;
            clientTimer += Time.DeltaTime;

            if (serverTimer >= 0.05f)
                server?.Broadcast();

            if (clientTimer >= 0.01f)
                client?.Sync();

            //foreach (GameObject gameObject in gameObjects)
            //{
            //    if (!gameObject.Active)
            //        continue;
            //    NetworkTransform netTransform = gameObject.GetComponent<NetworkTransform>();
            //    if (!netTransform || !netTransform.Active)
            //        continue;

            //    Transform transform = gameObject.GetComponent<Transform>();
            //    Position position = new Position(transform.Position.X, transform.Position.Y);
            //    byte[] data = Serializer.NetSerialize(position);
            //}
        }
    }

    public class Server
    {
        int id;
        int frame;
        Dictionary<Socket, Position> positions;
        NetworkServer server;

        public Server(int port)
        {
            id = 0;
            frame = 0;
            positions = new Dictionary<Socket, Position>();
            server = new NetworkServer(port);
            //注册回调
            server.OnConnected += OnConnected;
            server.OnDisconnected += OnDisconnected;
            server.Register((ushort)Role.Client, (ushort)Command.PosSync, PosSync);
        }

        private void OnConnected(Socket socket)
        {
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
            server.Send(socket, (ushort)Role.Server, (ushort)Command.StartSync, startSync);
        }

        private void OnDisconnected(Socket socket)
        {
            positions.Remove(socket);
        }

        private void PosSync(Socket socket, byte[] data)
        {
            C2S_PosSync posSync = Serializer.NetDeserialize<C2S_PosSync>(data);
            //修改数据
            positions[socket] = posSync.Position;
        }

        public void Broadcast()
        {
            foreach (var each in positions)
            {
                S2C_PosSync posSync = new S2C_PosSync();
                posSync.Frame = frame;
                posSync.Positions = new List<Position>();
                foreach (var pos in positions.Values)
                    posSync.Positions.Add(pos);

                server.Send(each.Key, (ushort)Role.Server, (ushort)Command.PosSync, posSync);
            }
        }
    }

    public class Client
    {
        bool start;
        Instantiate creatSelf;
        Instantiate creatOther;
        int id;
        int frame;
        GameObject self;
        List<GameObject> objects;
        List<Position> positions;
        NetworkClient client;

        public Client(Instantiate creatSelf, Instantiate creatOther, string ip, int port)
        {
            start = false;
            this.creatSelf = creatSelf;
            this.creatOther = creatOther;
            id = 0;
            frame = 0;
            self = null;
            objects = new List<GameObject>();
            positions = new List<Position>();
            client = new NetworkClient(ip, port);
            client.Register((ushort)Role.Server, (ushort)Command.StartSync, StartSync);
            client.Register((ushort)Role.Server, (ushort)Command.PosSync, PosSync);
        }

        private void StartSync(byte[] data)
        {
            S2C_StartSync startSync = Serializer.NetDeserialize<S2C_StartSync>(data);
            id = startSync.YourId;
            frame = startSync.Frame;
            positions = startSync.Positions;
            //创建self
            foreach (Position position in positions)
            {
                if (position.Id == id) //创建自己
                {
                    self = creatSelf();
                    self.transform.Position = new Vector2Int(position.X, position.Y);
                }
                else
                {
                    GameObject other = creatOther();
                    other.transform.Position = new Vector2Int(position.X, position.Y);
                    objects.Add(other);
                }
            }
            start = true;
        }

        private void PosSync(byte[] data)
        {
            S2C_PosSync posSync = Serializer.NetDeserialize<S2C_PosSync>(data);
            //更新位置
            frame = posSync.Frame;
            positions = posSync.Positions;

            foreach (var item in positions)
            {
                if (item.Id == id) //同步别人
                    continue;
                objects[item.Id].transform.Position = new Vector2Int(item.X, item.Y);
            }
        }

        public void Sync()
        {
            if (!start)
                return;
            C2S_PosSync posSync = new C2S_PosSync();
            posSync.Frame = frame;
            posSync.Position = new Position(id, self.transform.Position.X, self.transform.Position.Y);
            //同步自己坐标
            client.Send((ushort)Role.Client, (ushort)Command.PosSync, posSync);
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
    public class Position
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
    public class C2S_PosSync
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public Position Position;
    }

    [ProtoContract]
    public class S2C_PosSync
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public List<Position> Positions;
    }

    [ProtoContract]
    public class S2C_StartSync
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int YourId;
        [ProtoMember(3)]
        public List<Position> Positions;
    }
}