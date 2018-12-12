namespace Destroy.Net
{
    using System;
    using System.Net.Sockets;
    using System.Collections.Generic;
    using ProtoBuf;
    using Destroy;

    public static class NetworkSystem
    {
        private static Dictionary<int, Instantiate> prefabs;

        internal static void Init(bool useNet, int clientSyncRate, int serverBroadcastRate)
        {
            NetworkSystem.useNet = useNet;
            clientInterval = (float)1 / clientSyncRate;
            serverInterval = (float)1 / serverBroadcastRate;
        }

        public static void Register(Dictionary<int, Instantiate> prefabs) => NetworkSystem.prefabs = prefabs;

        public static Client Client;
        private static Server server;
        private static bool useNet;
        private static float clientInterval;
        private static float serverInterval;
        private static bool choose;
        private static float serverTimer;
        private static float clientTimer;

        internal static void Update(List<GameObject> gameObjects)
        {
            if (!useNet)
                return;
            if (prefabs == null)
                throw new Exception("Please Call NetworkSystem.Register if you useNet");

            if (!choose)
            {
                Console.WriteLine("1.Client, 2.Server");
                int mode = int.Parse(Console.ReadLine());
                switch (mode)
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
                choose = true;
                Console.Clear();
            }

            if (server != null)
            {
                serverTimer += Time.DeltaTime;
                server.Update();
                if (serverTimer >= serverInterval)
                {
                    serverTimer = 0;
                    server.Broadcast();
                }
            }
            if (Client != null)
            {
                clientTimer += Time.DeltaTime;
                Client.Update();
                if (clientTimer >= clientInterval)
                {
                    clientTimer = 0;
                    Client.Move();
                }
            }
        }
    }

    public class Server : NetworkServer
    {
        private int clientId;
        private int frame;
        private Dictionary<Socket, int> clients;                    //客户端, Id
        private Dictionary<int, Instantiate> prefabs;               //物体Id, Prefab
        private int instanceId;
        private Dictionary<int, Dictionary<int, Entity>> instances; //管理该局中的对象的位置

        public Server(int port, Dictionary<int, Instantiate> prefabs) : base(port)
        {
            clientId = 0;
            frame = 0;
            clients = new Dictionary<Socket, int>();
            this.prefabs = prefabs;
            instanceId = 0;
            instances = new Dictionary<int, Dictionary<int, Entity>>();
            foreach (var prefab in this.prefabs)
                instances.Add(prefab.Key, new Dictionary<int, Entity>());

            Console.WriteLine("服务器开启");

            base.OnConnected += OnConnected;
            base.OnDisconnected += OnDisconnected;
            Register((ushort)Role.Client, (ushort)Cmd.Move, OnMove);
            Register((ushort)Role.Client, (ushort)Cmd.Instantiate, OnInstantiate);
            Register((ushort)Role.Client, (ushort)Cmd.Destroy, OnDestroy);
        }

        public void Broadcast()
        {
            S2C_Move move = new S2C_Move();
            move.Frame = frame;
            move.Entities = new List<Entity>();
            frame++;
            foreach (var dict in instances.Values)
            {
                foreach (var entity in dict.Values)
                {
                    move.Entities.Add(entity);
                }
            }

            //广播所有人
            foreach (var client in clients.Keys)
                Send(client, (ushort)Role.Server, (ushort)Cmd.Move, move);
        }

        private new void OnConnected(Socket socket)
        {
            clients.Add(socket, clientId);
            Console.WriteLine("客户端连接");

            //发送断线重连包
            S2C_Join join = new S2C_Join();
            join.Frame = frame;
            join.YourId = clientId;
            join.Instances = new List<Instance>();
            foreach (var instance in instances)
            {
                foreach (var item in instance.Value) //该类型下的物体
                {
                    Instance instantiate = new Instance();
                    instantiate.IsLocal = clients[socket] == item.Key;
                    instantiate.TypeId = instance.Key;
                    instantiate.Id = item.Key;      //物品Id
                    instantiate.X = item.Value.X;
                    instantiate.Y = item.Value.Y;
                    //添加
                    join.Instances.Add(instantiate);
                }
            }
            //自增
            clientId++;
            //发送给登陆者
            Send(socket, (ushort)Role.Server, (ushort)Cmd.Join, join);
        }

        private new void OnDisconnected(string msg, Socket socket)
        {
            clients.Remove(socket);
            Console.WriteLine($"客户端断开连接:{msg}");
        }

        private void OnMove(Socket socket, byte[] data)
        {
            C2S_Move move = NetworkSerializer.Deserialize<C2S_Move>(data);

            if (move.Entities == null) //表示该玩家没有创建移动的游戏物体
                return;
            //玩家的实体
            foreach (var entity in move.Entities)
            {
                //获得场景中所有实例
                foreach (var dict in instances.Values)
                {
                    dict[entity.Id] = entity; //赋值
                }
            }
        }

        private void OnInstantiate(Socket socket, byte[] data)
        {
            C2S_Instantiate clientCmd = NetworkSerializer.Deserialize<C2S_Instantiate>(data);

            //新增对象数据
            instances[clientCmd.TypeId].Add(instanceId, new Entity(clients[socket], clientCmd.X, clientCmd.Y));

            S2C_Instantiate cmd = new S2C_Instantiate();
            cmd.Frame = clientCmd.Frame;
            cmd.Instance.TypeId = clientCmd.TypeId;
            cmd.Instance.Id = instanceId;
            cmd.Instance.X = clientCmd.X;
            cmd.Instance.Y = clientCmd.Y;
            //自增
            instanceId++;
            //广播所有人
            foreach (var client in clients.Keys)
            {
                if (client == socket) //调用者拥有IsLocal属性
                    cmd.Instance.IsLocal = true;
                else
                    cmd.Instance.IsLocal = false;
                Send(client, (ushort)Role.Server, (ushort)Cmd.Instantiate, cmd);
            }
        }

        private void OnDestroy(Socket socket, byte[] data)
        {
            C2S_Destroy clientCmd = NetworkSerializer.Deserialize<C2S_Destroy>(data);

            //移除对象数据
            instances[clientCmd.TypeId].Remove(clientCmd.Id);

            S2C_Destroy cmd = new S2C_Destroy();
            cmd.Frame = clientCmd.Frame;
            cmd.TypeId = clientCmd.TypeId;
            cmd.Id = clientCmd.Id;
            //广播除了发送者之外的人
            foreach (var client in clients.Keys)
            {
                if (client == socket)
                    continue;
                Send(client, (ushort)Role.Server, (ushort)Cmd.Destroy, cmd);
            }
        }
    }

    public class Client : NetworkClient
    {
        private bool join;
        private int id;
        private int frame;
        private Dictionary<int, Instantiate> prefabs;
        private Dictionary<int, Dictionary<int, GameObject>> instances; //管理该局游戏场景中的对象

        private Dictionary<int, GameObject> selfIntances; //物品Id, 游戏物体
        private Dictionary<int, GameObject> otherInstances;

        public Client(string serverIp, int serverPort, Dictionary<int, Instantiate> prefabs) : base(serverIp, serverPort)
        {
            join = false;
            id = -1;
            frame = -1;
            this.prefabs = prefabs;
            instances = new Dictionary<int, Dictionary<int, GameObject>>();
            foreach (var prefab in prefabs)
                instances.Add(prefab.Key, new Dictionary<int, GameObject>());
            //负责管理移动
            selfIntances = new Dictionary<int, GameObject>();
            otherInstances = new Dictionary<int, GameObject>();

            Register((ushort)Role.Server, (ushort)Cmd.Join, JoinCallback);
            Register((ushort)Role.Server, (ushort)Cmd.Move, MoveCallback);
            Register((ushort)Role.Server, (ushort)Cmd.Destroy, DestroyCallback);
            Register((ushort)Role.Server, (ushort)Cmd.Instantiate, OnInstantiated);
        }

        public void Move()
        {
            if (!join) //等待加入游戏成功
                return;

            C2S_Move move = new C2S_Move();
            move.Frame = frame;
            move.Entities = new List<Entity>();
            foreach (var instance in selfIntances)
            {
                Vector2Int pos = instance.Value.transform.Position;
                move.Entities.Add(new Entity(instance.Key, pos.X, pos.Y));
            }
            Send((ushort)Role.Client, (ushort)Cmd.Move, move);
        }

        public void Instantiate_RPC(int typeId, Vector2Int position)
        {
            if (!prefabs.ContainsKey(typeId))
                return;
            C2S_Instantiate cmd = new C2S_Instantiate();
            cmd.Frame = frame;
            cmd.TypeId = typeId;
            //修改坐标
            cmd.X = position.X;
            cmd.Y = position.Y;
            //发送指令
            Send((ushort)Role.Client, (ushort)Cmd.Instantiate, cmd);
        }

        public void Destroy(GameObject instance)
        {
            if (!instance) //如果没有游戏物体
                return;
            NetworkIdentity identity = instance.GetComponent<NetworkIdentity>();
            if (!identity || !identity.Active) //如果没有该组件或者不活跃
                return;
            //只允许删除自己的游戏物体
            if (!selfIntances.ContainsKey(identity.Id))
                return;

            //从列表中删除
            instances[identity.TypeId].Remove(identity.Id);
            //从场景中删除
            global::Destroy.Object.Destroy(instance);
            //从自己物体中删除
            selfIntances.Remove(identity.Id);

            C2S_Destroy cmd = new C2S_Destroy();
            cmd.Frame = frame;
            cmd.TypeId = identity.TypeId;
            cmd.Id = identity.Id;
            //发送指令
            Send((ushort)Role.Client, (ushort)Cmd.Destroy, cmd);
        }

        private void JoinCallback(byte[] data)
        {
            S2C_Join join = NetworkSerializer.Deserialize<S2C_Join>(data);
            this.join = true;
            frame = join.Frame;
            id = join.YourId;
            if (join.Instances == null) //表示当前没有游戏物体
                return;
            //生成所有游戏物体
            foreach (var instance in join.Instances)
                CreatInstance(instance);
        }

        private void MoveCallback(byte[] data)
        {
            S2C_Move move = NetworkSerializer.Deserialize<S2C_Move>(data);
            frame = move.Frame;
            if (move.Entities == null) //表示当前没有游戏物体
                return;
            //实现其他物体同步移动
            foreach (var entity in move.Entities)
            {
                //只同步他人
                Vector2Int pos = new Vector2Int(entity.X, entity.Y);
                if (otherInstances.ContainsKey(entity.Id))
                    otherInstances[entity.Id].transform.Position = pos;
            }
        }

        private void DestroyCallback(byte[] data)
        {
            S2C_Destroy cmd = NetworkSerializer.Deserialize<S2C_Destroy>(data);
            //获取场景实例
            GameObject instance = instances[cmd.TypeId][cmd.Id];
            //从列表中删除
            instances[cmd.TypeId].Remove(cmd.Id);
            //从场景中删除
            global::Destroy.Object.Destroy(instance);
            //从他人物体中移除
            otherInstances.Remove(cmd.Id);
        }

        private void OnInstantiated(byte[] data)
        {
            S2C_Instantiate cmd = NetworkSerializer.Deserialize<S2C_Instantiate>(data);
            CreatInstance(cmd.Instance);
        }

        private void CreatInstance(Instance instance)
        {
            //创建场景实例
            GameObject gameObject = prefabs[instance.TypeId]();
            //加进对象列表
            instances[instance.TypeId].Add(instance.Id, gameObject);
            //加进本地物体
            if (instance.IsLocal)
                selfIntances.Add(instance.Id, gameObject);
            //加进他人物体
            else
                otherInstances.Add(instance.Id, gameObject);

            //修改坐标
            gameObject.transform.Position = new Vector2Int(instance.X, instance.Y);
            //添加Id组件
            NetworkIdentity identity = gameObject.AddComponent<NetworkIdentity>();
            identity.TypeId = instance.TypeId;
            identity.Id = instance.Id;
            //组件赋值
            List<NetworkScript> netScripts = gameObject.GetComponents<NetworkScript>();
            foreach (NetworkScript netScript in netScripts)
                netScript.IsLocal = instance.IsLocal;
        }
    }

    public enum Role
    {
        Client,
        Server,
    }

    public enum Cmd
    {
        Join,
        Move,
        Instantiate,
        Destroy,
    }

    [ProtoContract]
    public struct C2S_Instantiate
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int TypeId;
        [ProtoMember(3)]
        public int X;
        [ProtoMember(4)]
        public int Y;
    }

    [ProtoContract]
    public struct S2C_Instantiate
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public Instance Instance;
    }

    [ProtoContract]
    public struct C2S_Destroy
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int TypeId;
        [ProtoMember(3)]
        public int Id;
    }

    [ProtoContract]
    public struct S2C_Destroy
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int TypeId;
        [ProtoMember(3)]
        public int Id;
    }

    [ProtoContract]
    public struct S2C_Join
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public int YourId;
        [ProtoMember(3)]
        public List<Instance> Instances;
    }

    [ProtoContract]
    public struct Instance
    {
        [ProtoMember(1)]
        public int TypeId;
        [ProtoMember(2)]
        public int Id;
        [ProtoMember(3)]
        public bool IsLocal;
        [ProtoMember(4)]
        public int X;
        [ProtoMember(5)]
        public int Y;
    }

    [ProtoContract]
    public struct Entity
    {
        [ProtoMember(1)]
        public int Id;
        [ProtoMember(2)]
        public int X;
        [ProtoMember(3)]
        public int Y;

        public Entity(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }

    [ProtoContract]
    public struct C2S_Move
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public List<Entity> Entities; //Self Instances's Postions
    }

    [ProtoContract]
    public struct S2C_Move
    {
        [ProtoMember(1)]
        public int Frame;
        [ProtoMember(2)]
        public List<Entity> Entities;
    }
}