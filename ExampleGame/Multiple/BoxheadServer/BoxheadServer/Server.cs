using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Destroy;
using Destroy.Net;
using Boxhead.Message;
using System.Collections.Concurrent;

public class Player
{
    public int Id;
    public Socket Socket;

    public Player(int id, Socket socket)
    {
        Id = id;
        Socket = socket;
    }
}

public class CallBack
{
    public void Excute()
    {

    }
}

[CreatGameObject]
public class Server : Script
{
    private int frameIndex;                                                      //游戏帧
    private int playerId;                                                        //玩家自增id

    private ConcurrentBag<Player> players;
    //玩家与他某一帧的操作的操作的集合
    private ConcurrentDictionary<Player, ConcurrentDictionary<int, C2S_Input>> playerFrameInputs;
    private ConcurrentQueue<CallBack> callBacks;
    private Socket serverSocket;

    public override void Start()
    {
        frameIndex = 0;
        playerId = 0;
        players = new ConcurrentBag<Player>();
        playerFrameInputs = new ConcurrentDictionary<Player, ConcurrentDictionary<int, C2S_Input>>();
        callBacks = new ConcurrentQueue<CallBack>();

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        EndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
        serverSocket.Bind(serverEndPoint);
        serverSocket.Listen(0);

        Thread accept = new Thread(Accept) { IsBackground = true };
        accept.Start();

        Thread handle = new Thread(Handle) { IsBackground = true };
        handle.Start();

        Console.WriteLine("服务器开启成功!");
    }

    void Accept()
    {
        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            string clientEndPoint = clientSocket.RemoteEndPoint.ToString();

            Player player = new Player(playerId++, clientSocket);
            players.Add(player);

            Console.WriteLine($"{clientEndPoint}连接成功!");

            Thread receive = new Thread(Receive) { IsBackground = true };
            receive.Start(player);
        }
    }

    void Handle()
    {
        while (true)
        {
            if (callBacks.Count > 0)
            {
                if(callBacks.TryDequeue(out CallBack callBack))
                {
                    callBack.Excute();
                }
            }
            Thread.Sleep(10);
        }
    }

    void Receive(object param)
    {
        Player player = (Player)param;

        while (true)
        {
            byte[] data = new byte[4];
            int length = 0;

        }
    }

    void Send()
    {
        while (true)
        {
            frameIndex++;
            Thread.Sleep(50); // 20 times per second
        }
    }
}