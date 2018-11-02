using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Destroy;

[CreatGameObject(2, "Client")]
public class Client : Script
{
    private Socket clientSocket;

    public override void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666); //TODO
        clientSocket.Connect(serverEP);
        Console.WriteLine("客户端连接成功");

        Thread receive = new Thread(Receive) { IsBackground = true };
        Thread send = new Thread(Send) { IsBackground = true };
        receive.Start();
        send.Start();
    }

    public override void Update(float deltaTime)
    {
        //TODO
    }

    void Receive()
    {
        while (true)
        {

        }
    }

    void Send()
    {
        while (true)
        {



            Thread.Sleep(10);
        }
    }
}