using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Destroy;
using Destroy.Net;
using Boxhead.Message;
using System.Collections.Generic;
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

    public void Execute() => @event(obj, data);
}

[CreatGameObject]
public class Client : Script
{
    // <帧, <id, input>>
    private Dictionary<int, Dictionary<int, PlayerInput>> playerFrameInputs;

    private int id;
    private int frameIndex;
    private int serverIndex;
    private List<Player> players;
    private ConcurrentQueue<byte[]> messages;
    private ConcurrentQueue<Callback> callbacks;
    private Dictionary<int, MessageEvent> messageEvents;
    private Socket clientSocket;

    void OnStartGame(object obj, byte[] data)
    {
        StartGame startGame = NetworkUtils.NetDeserialize<StartGame>(data);
        id = startGame.playerId;
        foreach (var id in startGame.players)
            players.Add(new Player(id));

        Console.WriteLine("My Id:" + id + "当前玩家人数" + players.Count);
    }

    void OnFrameSync(object obj, byte[] data)
    {
        FrameSync frameSync = NetworkUtils.NetDeserialize<FrameSync>(data);
        serverIndex = frameSync.frameIndex;
        playerFrameInputs = frameSync.playerInputs;

        Console.WriteLine(serverIndex);
    }

    public override void Start()
    {
        id = 0;
        frameIndex = -1;
        serverIndex = -1;
        players = new List<Player>();
        messages = new ConcurrentQueue<byte[]>();
        callbacks = new ConcurrentQueue<Callback>();
        messageEvents = new Dictionary<int, MessageEvent>();
        //register msg events
        Register(ActionType.Server, MessageType.StartGame, OnStartGame);
        Register(ActionType.Server, MessageType.FrameSync, OnFrameSync);

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666); //TODO
        clientSocket.Connect(serverEP);
        Console.WriteLine("客户端连接成功");

        Thread receive = new Thread(Receive) { IsBackground = true };
        receive.Start();
        Thread handle = new Thread(Handle) { IsBackground = true };
        handle.Start();
        Thread send = new Thread(Send) { IsBackground = true };
        send.Start();
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

    public override void Update(float deltaTime)
    {
        //当服务器走到第1帧时才开始调用
        if (frameIndex > -1 && serverIndex - frameIndex == 1)
        {
            //遍历所有玩家输入
            foreach (var each in playerFrameInputs.Values)
            {
                for (int i = 0; i < each.Count; i++)
                {
                    PlayerInput value = each[i]; // read
                    if (value != null && value.frameIndex == serverIndex)
                    {
                        Console.WriteLine($"玩家{i}按下了A:{value.left}");
                        each[i] = null; // write
                    }
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                PlayerInput input = new PlayerInput();
                input.frameIndex = frameIndex;
                input.left = true;
                byte[] data = MessageSerializer.SerializeMsg(ActionType.Client, MessageType.PlayerInput, input);
                messages.Enqueue(data);

                frameIndex++;
            }
        }
        else if (serverIndex - frameIndex == 2)
        {
            frameIndex++;
        }
    }

    void Receive()
    {
        while (true)
        {
            //Read & Serialize
            byte[] data = MessageSerializer.DeserializeMsg(clientSocket, out ActionType action, out MessageType type);
            int key = MessageSerializer.Enum2Int(action, type);

            if (messageEvents.ContainsKey(key))
            {
                var @event = messageEvents[key];
                Callback callback = new Callback(@event, null, data);
                callbacks.Enqueue(callback);
            }
        }
    }

    void Handle()
    {
        while (true)
        {
            if (callbacks.Count > 0)
            {
                if (callbacks.TryDequeue(out Callback callback))
                    callback.Execute();
            }
            Thread.Sleep(10);
        }
    }

    void Send()
    {
        while (true)
        {
            if (messages.Count > 0)
            {
                if (messages.TryDequeue(out byte[] data))
                    clientSocket.Send(data);
            }
            Thread.Sleep(10);
        }
    }
}