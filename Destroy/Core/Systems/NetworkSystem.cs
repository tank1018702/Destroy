namespace Destroy
{
    using System;
    using System.Net.Sockets;
    using System.Collections.Generic;
    using ProtoBuf;

    public static class NetworkSystem
    {
        //public static void Init()
        //{
        //    //    private static List<NetworkTransform> networkTransforms;
        //    //networkTransforms = new List<NetworkTransform>();
        //    //foreach (GameObject gameObject in gameObjects)
        //    //{
        //    //    if (!gameObject.Active)
        //    //        continue;
        //    //    NetworkTransform networkTransform = gameObject.GetComponent<NetworkTransform>();
        //    //    if (!networkTransform || !networkTransform.Active)
        //    //        continue;

        //    //    if (!networkTransforms.Contains(networkTransform))
        //    //        networkTransforms.Add(networkTransform);
        //    //}
        //}

        public static Client Client;
        private static Dictionary<int, Instantiate> prefabs;

        public static void Init(Dictionary<int, Instantiate> prefabs)
        {
            NetworkSystem.prefabs = prefabs;
        }

        private static bool choose = false;
        private static Server server;
        private static float serverTimer;
        private static float clientTimer;

        internal static void Update(List<GameObject> gameObjects)
        {
            //return;
            if (!choose)
            {
                choose = true;
                Print.DrawLine("1.client, 2.server", ConsoleColor.White);
                //Get Input
                switch (int.Parse(Console.ReadLine()))
                {
                    case 1:
                        Client = new Client(NetworkUtils.LocalIPv4Str, 8848, prefabs);
                        Client.Start();
                        break;
                    case 2:
                        server = new Server(8848, prefabs);
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
            }

            if (Client != null)
            {
                clientTimer += Time.DeltaTime;
                Client.Update();          //客户端每帧刷新
            }
        }
    }

    public class Server : NetworkServer
    {
        private Dictionary<int, Instantiate> prefabs;
        private int incrementalId;
        private int frame;
        private List<Socket> clients;

        public Server(int port, Dictionary<int, Instantiate> prefabs) : base(port)
        {
            this.prefabs = prefabs;
            incrementalId = 0;
            frame = 0;
            clients = new List<Socket>();
            base.OnConnected += OnConnected;
            base.OnDisconnected += OnDisconnected;
            Register((ushort)Role.Client, (ushort)Cmd.Instantiate, OnInstantiate);
            Register((ushort)Role.Client, (ushort)Cmd.Destroy, OnDestroy);
        }

        private new void OnConnected(Socket socket)
        {
            clients.Add(socket);
            Console.WriteLine("客户端连接");
        }

        private new void OnDisconnected(string msg, Socket socket)
        {
            clients.Remove(socket);
            Console.WriteLine($"客户端断开连接{msg}");
        }

        private void OnInstantiate(Socket socket, byte[] data)
        {
            Cmd_Instantiate instantiate = Serializer.NetDeserialize<Cmd_Instantiate>(data);

            foreach (var client in clients)
            {
                if (client == socket) //不发送自己
                    continue;
                //发送 (需要优化, 去除序列化)
                Send(client, (ushort)Role.Server, (ushort)Cmd.Instantiate, instantiate);
            }
        }

        private void OnDestroy(Socket socket, byte[] data)
        {
            Cmd_Destroy destroy = Serializer.NetDeserialize<Cmd_Destroy>(data);

            foreach (var client in clients)
            {
                if (client == socket) //不发送自己
                    continue;
                //发送 (需要优化, 去除序列化)
                Send(client, (ushort)Role.Server, (ushort)Cmd.Destroy, destroy);
            }
        }
    }

    public class Client : NetworkClient
    {
        private bool start;
        private int id;
        private int frame;
        private Dictionary<int, Instantiate> prefabs;        //ObjId, Prefab
        private Dictionary<int, List<GameObject>> instances; //ObjId, List<GameObject>

        public Client(string serverIp, int serverPort, Dictionary<int, Instantiate> prefabs) :
            base(serverIp, serverPort)
        {
            start = false;
            id = -1;
            frame = -1;
            this.prefabs = prefabs;
            //Init Instances
            instances = new Dictionary<int, List<GameObject>>();
            foreach (var prefab in prefabs)
                instances.Add(prefab.Key, new List<GameObject>());
            //注册回调
            base.OnConnected += OnConnected;
            Register((ushort)Role.Server, (ushort)Cmd.Instantiate, OnInstantiate);
            Register((ushort)Role.Server, (ushort)Cmd.Destroy, OnDestroy);
        }

        private new void OnConnected(Socket socket)
        {
            Instantiate(1);
        }

        public void Instantiate(int objId)
        {
            if (!prefabs.ContainsKey(objId)) //不存在该Id
                return;
            //创建本地场景实例
            GameObject localInstance = prefabs[objId]();
            //添加进列表
            instances[objId].Add(localInstance);
            //同步实例
            Cmd_Instantiate instantiate = new Cmd_Instantiate();
            instantiate.Frame = frame;
            instantiate.ObjId = objId;
            Send((ushort)Role.Client, (ushort)Cmd.Instantiate, instantiate);
        }

        public void Destroy(int objId, string objName)
        {
            if (!prefabs.ContainsKey(objId)) //不存在该Id
                return;
            List<GameObject> list = instances[objId];
            for (int i = 0; i < list.Count; i++)
            {
                GameObject gameObject = list[i];
                if (gameObject.Name == objName)
                {
                    Object.Destroy(gameObject); //从场景中删除
                    list.Remove(gameObject);    //从列表中删除
                    break;
                }
            }
            //同步销毁
            Cmd_Destroy destroy = new Cmd_Destroy();
            destroy.Frame = frame;
            destroy.ObjId = objId;
            destroy.ObjName = objName;
            Send((ushort)Role.Client, (ushort)Cmd.Destroy, destroy);
        }

        private void OnInstantiate(byte[] data)
        {
            Cmd_Instantiate instantiate = Serializer.NetDeserialize<Cmd_Instantiate>(data);
            Instantiate method = prefabs[instantiate.ObjId];
            //在场景中创建该游戏物体
            GameObject instance = method();
            //添加进列表
            instances[instantiate.ObjId].Add(instance);
        }

        private void OnDestroy(byte[] data)
        {
            Cmd_Destroy destroy = Serializer.NetDeserialize<Cmd_Destroy>(data);
            List<GameObject> list = instances[destroy.ObjId];
            for (int i = 0; i < list.Count; i++)
            {
                GameObject gameObject = list[i];
                if (gameObject.Name == destroy.ObjName)
                {
                    Object.Destroy(gameObject); //从场景中删除
                    list.Remove(gameObject);    //从列表中删除
                    break;
                }
            }
        }
    }

    [ProtoContract]
    public struct Cmd_Instantiate
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int ObjId;
    }

    [ProtoContract]
    public struct Cmd_Destroy
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int ObjId;
        [ProtoMember(3)]
        public string ObjName;
    }

    public enum Role
    {
        Client,
        Server
    }

    public enum Cmd
    {
        Instantiate,
        Destroy,
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