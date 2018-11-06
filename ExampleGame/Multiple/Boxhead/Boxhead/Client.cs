using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Destroy;
using Destroy.Net;
using Boxhead.Message;
using System.Collections.Generic;

[CreatGameObject]
public class Client : Script
{
    private Dictionary<int, MessageEvent> messageEvents;
    private Socket clientSocket;
    private int id;

    void OnStartGame(object obj, byte[] data)
    {
        StartGame startGame = NetworkUtils.Deserialize<StartGame>(data);
        id = startGame.id;
        Console.WriteLine(id);
    }

    public override void Start()
    {
        messageEvents = new Dictionary<int, MessageEvent>();
        //register msg events
        int key = MessageSerializer.Enum2Int(ActionType.Server, MessageType.StartGame);
        messageEvents.Add(key, OnStartGame);

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666); //TODO
        clientSocket.Connect(serverEP);
        Console.WriteLine("客户端连接成功");

        Thread receive = new Thread(Receive) { IsBackground = true };
        receive.Start();
        //Thread send = new Thread(Send) { IsBackground = true };
        //send.Start();
    }

    public override void Update(float deltaTime)
    {
        //TODO
    }

    void Receive()
    {
        while (true)
        {
            byte[] data = MessageSerializer.DeserializeMsg(clientSocket, out ActionType action, out MessageType type);
            int key = MessageSerializer.Enum2Int(action, type);
            if (messageEvents.ContainsKey(key))
            {
                MessageEvent messageEvent = messageEvents[key];
                messageEvent(null, data);
            }
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