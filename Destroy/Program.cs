namespace Destroy
{
    //public class A : Script
    //{
    //    Camera c;

    //    public override void Start()
    //    {
    //        GameObject camera = new GameObject();
    //        c = camera.AddComponent<Camera>();
    //        c.CharWidth = 2;
    //        RendererSystem.Init(camera);

    //        Renderer renderer = AddComponent<Renderer>();
    //        renderer.Order = 0;
    //        renderer.Str = "吊";
    //        renderer.ForeColor = ConsoleColor.Red;

    //        GameObject go = new GameObject();
    //        go.transform.Position = new Vector2Int(0, 0);
    //        Renderer renderer2 = go.AddComponent<Renderer>();
    //        renderer2.Order = 1;
    //        renderer2.Str = "2";
    //        renderer2.ForeColor = ConsoleColor.Blue;
    //    }

    //    public override void Update()
    //    {
    //        if (Input.GetKey(KeyCode.A))
    //            transform.Translate(Vector2Int.Left);
    //        if (Input.GetKey(KeyCode.D))
    //            transform.Translate(Vector2Int.Right);
    //        if (Input.GetKey(KeyCode.W))
    //            transform.Translate(Vector2Int.Up);
    //        if (Input.GetKey(KeyCode.S))
    //            transform.Translate(Vector2Int.Down);
    //        c.Center(gameObject);
    //    }
    //}


    ///// <summary>
    ///// 矩形碰撞检测
    ///// </summary>
    //private static bool RectIntersects(Vector2Int selfPos, Vector2Int otherPos, RectCollider self, RectCollider other)
    //{
    //    if (selfPos.X >= otherPos.X + other.Size.X || otherPos.X >= selfPos.X + self.Size.X)
    //        return false;
    //    else if (selfPos.Y >= otherPos.Y + other.Size.Y || otherPos.Y >= selfPos.Y + self.Size.Y)
    //        return false;
    //    else
    //        return true;
    //}

    using System;
    using System.Net.Sockets;
    using System.Threading;
    using ProtoBuf;

    //[ProtoContract]
    //public class Msg
    //{
    //    [ProtoMember(1)]
    //    public string Str;
    //}

    //[CreatGameObject]
    //public class Test : Script
    //{
    //    private void GetMsg(Socket socket, byte[] data)
    //    {
    //        Msg msg = Serializer.NetDeserialize<Msg>(data);
    //        Console.WriteLine(Thread.CurrentThread.Name + " " + msg.Str);
    //    }

    //    Client client;

    //    public override void Start()
    //    {
    //        client = new Client(NetworkUtils.LocalIPv4Str, 8848);
    //        Server server = new Server(8848);

    //        NetworkSystem.Init(server, client);
    //        server.Register(0, 0, GetMsg);
    //    }

    //    public override void Update()
    //    {
    //        client.Send(0, 0, new Msg { Str = "A" });
    //    }
    //}

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine(new RuntimeDebugger());
            runtimeEngine.Run(20);
        }
    }
}