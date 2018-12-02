namespace Destroy.Test
{
    using System;
    using System.Net.Sockets;
    using Destroy.Net;
    using ProtoBuf;

    [ProtoContract]
    public class Msg
    {
        [ProtoMember(1)]
        public string Str;
    }

    [CreatGameObject]
    public class C : Script
    {
        private void GetMsg(Socket socket, byte[] data)
        {
            Msg msg = global::Destroy.Serializer.NetDeserialize<Msg>(data);
            Console.WriteLine(msg.Str);
        }

        Client client;

        public override void Start()
        {
            client = new Client(NetworkUtils.LocalIPv4Str, 8848);
            Server server = new Server(8848);

            NetworkSystem.Init(server, client);
            server.Register(0, 0, GetMsg);
        }

        public override void Update()
        {
            client.Send(0, 0, new Msg { Str = "A" });
        }
    }
}