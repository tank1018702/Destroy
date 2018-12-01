namespace Destroy.Test
{
    using Destroy.Net;
    using ProtoBuf;
    using System.Net.Sockets;

    [ProtoContract]
    public class Msg
    {
        [ProtoMember(1)]
        public string Str;
    }

    [CreatGameObject]
    public class C : Script
    {
        public override void Start()
        {
            Client client = new Client();
            client.Connect(NetworkUtils.LocalIPv4Str, 8848);
            client.Send(0, 0, new Msg { Str = "A" });
        }
    }

    [CreatGameObject(0)]
    public class S : Script
    {
        private void GetMsg(Socket socket, byte[] data)
        {
            Msg msg = global::Destroy.Serializer.NetDeserialize<Msg>(data);
            System.Console.WriteLine(msg.Str);
        }

        public override void Start()
        {
            Server server = new Server(8848);
            server.Register(0, 0, GetMsg);
            server.Start();
        }
    }
}