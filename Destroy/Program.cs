namespace Destroy
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Destroy.Net;
    using System.Net.Sockets;

    //[CreatGameObject]
    //public class A : Script
    //{
    //    public override void Start()
    //    {
    //        GameObject camera = new GameObject();
    //         camera.AddComponent<Camera>();
    //        RendererSystem.Init(camera);

    //        Renderer renderer = AddComponent<Renderer>();
    //        renderer.Order = 0;
    //        renderer.Str = "正";
    //        renderer.Width = 3;
    //        renderer.ForeColor = ConsoleColor.Red;

    //        GameObject go = new GameObject();
    //        go.transform.Position = new Vector2Int(0, 0);
    //        Renderer renderer2 = go.AddComponent<Renderer>();
    //        renderer2.Order = 1;
    //        renderer2.Str = "2";
    //        renderer2.Width = 1;
    //        renderer2.ForeColor = ConsoleColor.Blue;
    //    }

    //    public override void Update()
    //    {
    //        if (Input.GetKey(KeyCode.A))
    //            transform.Translate(new Vector2Int(-1, 0));
    //        if (Input.GetKey(KeyCode.D))
    //            transform.Translate(new Vector2Int(1, 0));
    //        if (Input.GetKey(KeyCode.S))
    //            transform.Translate(new Vector2Int(0, -1));
    //        if (Input.GetKey(KeyCode.W))
    //            transform.Translate(new Vector2Int(0, 1));
    //    }
    //}

    [CreatGameObject]
    public class N : Script
    {
        public void OnHello(object obj, byte[] data)
        {
            Hello hello = Serializer.NetDeserialize<Hello>(data);
            string a = (string)obj;
            Console.WriteLine(a + "   " + hello.Msg);
        }

        public override void Start()
        {
            Server server = new Server(8848);
            server.Register(SenderType.Client, MessageType.Hello, OnHello);
            server.Start();
            Client client = new Client(NetworkUtils.LocalIPv4Str, 8848);
            client.Connect();
            client.Send(SenderType.Client, MessageType.Hello, new Hello() { Msg = "123" });
        }
    }

    public class Client : NetworkClient
    {
        public Client(string serverIp, int serverPort) : base(serverIp, serverPort)
        {
        }

        protected override void OnConnected()
        {
            Console.WriteLine("连接成功");
        }
    }

    public class Server : NetworkServer
    {
        public Server(int port) : base(port)
        {
        }

        protected override object OnAccept(TcpClient tcpClient)
        {
            return "Server";
        }
    }

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine(new RuntimeDebugger());
            runtimeEngine.Run(20);
        }
    }
}