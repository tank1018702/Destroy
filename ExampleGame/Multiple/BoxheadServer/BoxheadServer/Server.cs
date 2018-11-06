using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Destroy;
using Destroy.Net;
using Boxhead.Message;
using System.Collections.Concurrent;

public delegate void Callback(Player player, byte[] data);

public class Event
{
    public void Excute()
    {

    }
}

public class Message
{

}

[CreatGameObject]
public class Server : Script
{
    private int frameIndex;                   //游戏帧
    private int playerId;                     //玩家自增id
    private ConcurrentBag<Player> players;    //玩家集合
    //所有玩家在某一帧的操作集合
    private ConcurrentDictionary<Player, ConcurrentDictionary<int, PlayerInput>> playerFrameInputs;

    private Queue<Callback> callbacks;           //注册回调事件
    private ConcurrentQueue<Message> messages;   //待发送消息队列
    private ConcurrentQueue<Event> callBacks;    //待处理事件队列
    private Socket serverSocket;                 //服务器套接字

    public override void Start()
    {
        frameIndex = 0;
        playerId = 0;
        players = new ConcurrentBag<Player>();
        playerFrameInputs = new ConcurrentDictionary<Player, ConcurrentDictionary<int, PlayerInput>>();
        callBacks = new ConcurrentQueue<Event>();

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        EndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
        serverSocket.Bind(serverEndPoint);
        serverSocket.Listen(0);

        Thread accept = new Thread(Accept) { IsBackground = true };
        accept.Start();
        Thread send = new Thread(Send) { IsBackground = true };
        send.Start();
        Thread handle = new Thread(Handle) { IsBackground = true };
        handle.Start();

        Console.WriteLine("服务器开启成功!");
    }

    public void Register(Callback callback)
    {

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

    void Send()
    {
        while (true)
        {
            frameIndex++;
            Thread.Sleep(50); // 20 times per second
        }
    }

    void Handle()
    {
        while (true)
        {
            if (callBacks.Count > 0)
            {
                if (callBacks.TryDequeue(out Event callBack))
                {
                    callBack.Excute();
                }
            }
            Thread.Sleep(10);
        }
    }

    /// <summary>
    /// 每个玩家对应一个收包线程
    /// </summary>
    /// <param name="param"></param>
    void Receive(object param)
    {
        Player player = (Player)param;

        while (true)
        {
            byte[] head = new byte[4];

            int length = 0;                      //包长度
            MessageType type = MessageType.None; //消息类型


            try
            {
                player.Socket.Receive(head);         //一定收4字节的包


            }
            catch (Exception)
            {
                player.Socket.Close();
                bool suc = players.TryTake(out player);
                Console.WriteLine($"收包异常, 移除玩家{player.Id}:{suc}");
                return; //结束该线程
            }

        }
    }
}