using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Destroy;
using Destroy.Net;
using Boxhead.Message;
using System.Collections.Concurrent;

public class Callback
{
    private object obj;
    private byte[] data;
    private MessageEvent @event;

    public Callback(MessageEvent @event, object obj, byte[] data)
    {
        this.@event = @event;
        this.obj = obj;
        this.data = data;
    }

    public void Excute() => @event(obj, data);
}

[CreatGameObject]
public class Server : Script
{
    private int frameIndex;                   //游戏帧
    private int playerId;                     //玩家自增id
    private ConcurrentBag<Player> players;    //玩家集合
    //所有玩家在某一帧的操作集合
    private ConcurrentDictionary<int, ConcurrentDictionary<int, PlayerInput>> playerFrameInputs;

    private Dictionary<int, MessageEvent> messageEvents; //注册回调事件
    private ConcurrentQueue<Callback> callbacks;         //待处理事件队列
    private Socket serverSocket;                         //服务器套接字

    public override void Start()
    {
        frameIndex = 0;
        playerId = 0;
        players = new ConcurrentBag<Player>();
        playerFrameInputs = new ConcurrentDictionary<int, ConcurrentDictionary<int, PlayerInput>>();

        messageEvents = new Dictionary<int, MessageEvent>();
        callbacks = new ConcurrentQueue<Callback>();

        //Register Events
        Register(ActionType.Client, MessageType.PlayerInput, OnPlayerInput);

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

    public void Register(ActionType action, MessageType type, MessageEvent @event)
    {
        int key = MessageSerializer.Enum2Int(action, type);
        if (messageEvents.ContainsKey(key))
        {
            Debug.Error("不能添加重复key!");
            return;
        }
        messageEvents.Add(key, @event);
    }

    void OnPlayerInput(object obj, byte[] data)
    {
        Player player = (Player)obj;
        PlayerInput playerInput = NetworkUtils.NetDeserialize<PlayerInput>(data);

        //加上提前量并判断客户端网络延迟
        playerInput.frameIndex += 5;
        if (playerInput.frameIndex < frameIndex)
            return; //丢掉延迟超过一定时间的包

        player.Inputs.Enqueue(playerInput); //放入Player的输入集合

        Console.WriteLine("get input");
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

            //达到两人开始游戏
            if (players.Count == 2)
            {
                //broadcast
                foreach (var each in players)
                {
                    StartGame startGame = new StartGame();
                    startGame.playerId = each.Id;
                    startGame.players = new List<int>();
                    foreach (var p in players)
                    {
                        startGame.players.Add(p.Id);
                    }
                    //给客户端发送所有玩家的Id
                    byte[] data = MessageSerializer.SerializeMsg(ActionType.Server, MessageType.StartGame, startGame);
                    each.Socket.Send(data);
                }

                //开始帧同步
                Thread send = new Thread(Send) { IsBackground = true };
                send.Start();

                //每个玩家对应一个收包线程
                foreach (var each in players)
                {
                    Thread receive = new Thread(Receive) { IsBackground = true };
                    receive.Start(each);
                }
            }
        }
    }

    void Send()
    {
        while (true)
        {
            foreach (var player in players)
            {
                ConcurrentDictionary<int, PlayerInput> dict = new ConcurrentDictionary<int, PlayerInput>();
                if (player.Inputs.Count > 0)
                {
                    player.Inputs.TryDequeue(out PlayerInput playerInput);
                    dict.TryAdd(playerInput.frameIndex, playerInput); //加上了提前了的输入
                    Console.WriteLine("123");
                }
                else
                {
                    dict.TryAdd(frameIndex, null); //这帧没有输入
                }
                playerFrameInputs.TryAdd(player.Id, dict);
            }

            FrameSync frameSync = new FrameSync();
            frameSync.frameIndex = frameIndex;
            frameSync.playerInputs = playerFrameInputs;
            byte[] data = MessageSerializer.SerializeMsg(ActionType.Server, MessageType.FrameSync, frameSync);
            //Frame Sync Broadcast
            foreach (var player in players)
                player.Socket.Send(data);

            frameIndex++;

            Thread.Sleep(50); // 20 times per second
        }
    }

    void Handle()
    {
        while (true)
        {
            if (callbacks.Count > 0)
            {
                if (callbacks.TryDequeue(out Callback callBack))
                    callBack.Excute();
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
            //Read & Serialize
            byte[] data = MessageSerializer.DeserializeMsg(player.Socket, out ActionType action, out MessageType type);
            int key = MessageSerializer.Enum2Int(action, type);

            if (messageEvents.ContainsKey(key))
            {
                var @event = messageEvents[key];
                Callback callback = new Callback(@event, player, data);
                callbacks.Enqueue(callback);
            }
        }
    }
}