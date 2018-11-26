using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using Message;
using PBMessage;
using NetworkTools;
using System.Net;

public class RoomManager
{
    public Dictionary<uint, Room> Rooms; //所有房间
    public List<uint> GamingRooms;       //游戏中的房间

    private RoomManager()
    {
        Rooms = new Dictionary<uint, Room>();
        GamingRooms = new List<uint>();

        Room.CloseRoom += id =>
        {
            Rooms.Remove(id);
            GamingRooms.Remove(id);
        };
        Room.StartGame += id =>
        {
            GamingRooms.Add(id);
        };
    }

    private static RoomManager _instance;

    public static RoomManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new RoomManager();
            return _instance;
        }
    }

    public void CreatRoom(uint id)
    {
        if (!Rooms.ContainsKey(id))
        {
            Room room = new Room(id);
            Rooms.Add(id, room);
        }
    }

    public void RemoveRoom(uint id)
    {
        if (!Rooms.ContainsKey(id))
            return;
        Rooms[id].Close();
    }
}

public class Room
{
    public enum State
    {
        None,  //房间未开启
        Await, //等待中
        Ready, //达到最大开始人数
        Full,  //达到房间最大人数
        Gaming //游戏中
    }

    public const int PLAYER_LIMIT = 10;         //最大玩家人数
    public const int OB_LIMIT = 2;              //最大观察者人数

    public static event Action<uint> CloseRoom; //关闭房间事件
    public static event Action<uint> StartGame; //开始游戏事件

    public uint RoomId;                         //房间Id
    public NetworkGameplay Gameplay;            //游戏逻辑
    public State CurState;                      //当前状态
    public List<NetworkPlayer> Players;         //玩家集合
    public List<NetworkPlayer> OBs;             //观察者集合

    public Room(uint roomId)
    {
        RoomId = roomId;
        Gameplay = new NetworkGameplay();
        CurState = State.None;
        Players = new List<NetworkPlayer>();
        OBs = new List<NetworkPlayer>();
    }

    public void Start()
    {
        if (CurState != State.Ready || CurState != State.Full)
            return;
        CurState = State.Gaming;
        StartGame(RoomId);
    }

    public void GetIn(NetworkPlayer networkPlayer)
    {
        if (CurState == State.None || CurState == State.Full)
            return;

        networkPlayer.RoomId = RoomId;
        networkPlayer.InRoom = true;

        //优先进入玩家集合
        if (Players.Count < PLAYER_LIMIT)
        {
            Players.Add(networkPlayer);
            networkPlayer.CurType = NetworkPlayer.PlayerType.Player;
            if (Players.Count == PLAYER_LIMIT)
                CurState = State.Ready;
            else
                CurState = State.Await;
        }
        else if (OBs.Count < OB_LIMIT)
        {
            OBs.Add(networkPlayer);
            networkPlayer.CurType = NetworkPlayer.PlayerType.OB;
            if (OBs.Count == OB_LIMIT)
                CurState = State.Full;
        }
    }

    public void KickOut(NetworkPlayer networkPlayer)
    {
        if (CurState == State.None)
            return;

        networkPlayer.RoomId = null;
        networkPlayer.InRoom = false;

        if (Players.Contains(networkPlayer))
        {
            Players.Remove(networkPlayer);
            networkPlayer.CurType = NetworkPlayer.PlayerType.None;

            if (CurState == State.Ready || CurState == State.Full)
                CurState = State.Await;
        }
        else if (OBs.Contains(networkPlayer))
        {
            OBs.Remove(networkPlayer);
            networkPlayer.CurType = NetworkPlayer.PlayerType.None;

            if (CurState == State.Full)
                CurState = State.Ready;
        }
        //房间关闭
        if (Players.Count == 0)
        {
            Close();
        }
    }

    public void Close()
    {
        foreach (var each in Players)
        {
            KickOut(each);
        }
        foreach (var each in OBs)
        {
            KickOut(each);
        }
        CloseRoom(RoomId);
    }
}

/// <summary>
/// 用于在服务器与客户端保存玩家的状态与一定的数据
/// </summary>
public class NetworkPlayer
{
    public enum PlayerType
    {
        None,   //未加入房间
        Player, //玩家
        OB      //观察者
    }

    //客户端掉线事件
    public event Action<int> CloseEvent;

    //其他属性
    public TcpClient Client;
    public NetworkStream Stream;
    public bool ReceivedHeartbeat;
    public float LastSendTime;  //该玩家最后一次发送信息的时间

    //玩家信息
    public int NetworkId;
    public string Name;

    //玩家状态
    public bool Online;
    public uint? RoomId;
    public bool InRoom;
    public PlayerType CurType;

    public NetworkPlayer(TcpClient tcpClient, NetworkStream stream, Action<int> closeEvent = null)
    {
        CloseEvent = closeEvent;

        Client = tcpClient;
        Stream = stream;
        ReceivedHeartbeat = false;
        LastSendTime = 0;

        NetworkId = 0;
        Name = null;

        Online = false;
        RoomId = null;
        InRoom = false;
        CurType = PlayerType.None;
    }

    /// <summary>
    /// 断开与服务器的连接
    /// </summary>
    public void Close()
    {
        if (!Online)
            return;

        CloseEvent?.Invoke(NetworkId);   //有就执行掉线事件

        Stream.Close();
        Stream.Dispose();

        Client.Close();
        Client.Dispose();

        Online = false; //下线
    }
}

/// <summary>
/// 服务器回调
/// </summary>
public delegate void ServerCallBack(NetworkPlayer networkPlayer, byte[] data);

public class MessageHandler
{
    private NetworkPlayer _networkPlayer;
    private Msg _msg;
    private uint _timeStamp;
    private byte[] _data;

    public MessageHandler(NetworkPlayer player, Msg msg, uint timeStamp, byte[] data)
    {
        _networkPlayer = player;
        _msg = msg;
        _timeStamp = timeStamp;
        _data = data;
    }

    public IEnumerator Send()
    {
        byte[] data = MsgConverter.Pack(_msg, _timeStamp, _data);

        IAsyncResult async = _networkPlayer.Stream.BeginWrite(data, 0, data.Length, null, null);
        while (!async.IsCompleted)
        {
            yield return null;
        }
        try
        {
            _networkPlayer.Stream.EndWrite(async);
        }
        catch (Exception ex)
        {
            Debug.Log("消息发送失败" + ex.Message);
        }
    }
}

public class CallBackHandler
{
    private NetworkPlayer _networkPlayer;

    private ServerCallBack _serverCallBack;

    private byte[] _data;

    public CallBackHandler(NetworkPlayer networkPlayer, ServerCallBack serverCallBack, byte[] data)
    {
        _networkPlayer = networkPlayer;
        _serverCallBack = serverCallBack;
        _data = data;
    }

    public void Execute() => _serverCallBack(_networkPlayer, _data);
}

/// <summary>
/// 网络服务器
/// </summary>
public class NetworkServer : SingleTon<NetworkServer>
{
    public event Action<NetworkPlayer> ConnectResponseEvent;                      //玩家上线发送信息事件
    public event Action ServerOfflineEvent;                                       //连接失败事件
    public event Action<NetworkPlayer> CheckHeartbeatEvent;                       //检测心跳包事件

    public NetworkGameplay Gameplay;                                              //游戏逻辑
    public RoomManager RoomManager;                                               //房间管理器

    private int _networkIdIndex;                                                  //网络玩家下标, 从1开始
    public Dictionary<int, NetworkPlayer> NetworkPlayersDict;                     //网络玩家字典(保存在线玩家)

    private Dictionary<Msg, ServerCallBack> _callBacks;                           //回调事件

    private TcpListener _listener;                                                //服务器Socket
    private Queue<CallBackHandler> _callBackHandlers;                             //待处理队列
    private Queue<MessageHandler> _messageHandlers;                               //待发送队列

    public float TimeLine { get; private set; }                                   //服务器已连接时间
    public uint TimeStamp { get { return TimeConverter.Float2UInt(TimeLine); } }  //时间戳

    /// <summary>
    /// 注册回调事件
    /// </summary>
    public void Register(Msg msg, ServerCallBack callBack)
    {
        if (_callBacks == null)
            _callBacks = new Dictionary<Msg, ServerCallBack>();

        if (!_callBacks.ContainsKey(msg))
            _callBacks.Add(msg, callBack);
        else
            Debug.Log("注册了相同回调事件");
    }

    /// <summary>
    /// 开启服务器 <see langword="直接调用"/>
    /// </summary>
    public void Connect(string address, int port)
    {
        //客户端连接回应事件
        ConnectResponseEvent += networkPlayer =>
        {
            //向玩家发送NetworkId
            ConnectResponse response = new ConnectResponse
            {
                NetworkId = networkPlayer.NetworkId
            };
            byte[] data = PBConverter.Serializer(response);
            //发送id
            Send(networkPlayer, new Msg(ActType.Response, CmdType.Connect), data);
        };
        //服务器下线事件
        ServerOfflineEvent += () =>
        {
            Debug.LogWarning("服务器已关闭");
        };

        Gameplay = new NetworkGameplay();
        RoomManager = RoomManager.Instance;

        _networkIdIndex = 1;       //下标从1开始
        NetworkPlayersDict = new Dictionary<int, NetworkPlayer>();

        _listener = new TcpListener(IPAddress.Parse(address), port);
        _callBackHandlers = new Queue<CallBackHandler>();
        _messageHandlers = new Queue<MessageHandler>();

        _listener.Start(); //开启监听

        //计算时间协程
        StartCoroutine(_Timer());

        //开启三个协程负责处理客户端的消息
        StartCoroutine(_Await());
        StartCoroutine(_MessageHandler());
        StartCoroutine(_CallBackHandler());
    }

    private IEnumerator _Timer()
    {
        TimeLine = Time.time;
        while (true)
        {
            TimeLine += Time.fixedDeltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 加入消息队列等待发送 <see langword="低优先级"/>
    /// </summary>
    public void Enqueue(NetworkPlayer player, Msg msg, byte[] data = null)
    {
        MessageHandler handler = new MessageHandler(player, msg, TimeStamp, data);
        _messageHandlers.Enqueue(handler);
    }

    /// <summary>
    /// 开启协程发送消息 <see langword="高优先级"/>
    /// </summary>
    public void Send(NetworkPlayer player, Msg msg, byte[] data = null)
    {
        MessageHandler handler = new MessageHandler(player, msg, TimeStamp, data);
        StartCoroutine(handler.Send());
    }

    /// <summary>
    /// 等待客户端连接 <see langword="单开"/>
    /// </summary>
    private IEnumerator _Await()
    {
        TcpClient client = null;
        NetworkStream stream = null;

        while (true)
        {
            IAsyncResult async = _listener.BeginAcceptTcpClient(null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            try
            {
                client = _listener.EndAcceptTcpClient(async);
                stream = client.GetStream();
            }
            catch (Exception ex)
            {
                Debug.Log("服务器检测到:一个客户端连接失败" + ex.Message);
                continue;
            }

            //添加进客户端
            NetworkPlayer networkPlayer = new NetworkPlayer(client, stream, networkId => NetworkPlayersDict.Remove(networkId))
            {
                //赋值NetworkId
                NetworkId = _networkIdIndex,
                Online = true
            };
            //添加进字典管理
            NetworkPlayersDict.Add(networkPlayer.NetworkId, networkPlayer);

            //发送回应
            ConnectResponseEvent(networkPlayer);

            //开启监听协程
            StartCoroutine(_Receive(networkPlayer));
        }
    }

    /// <summary>
    /// 发送消息 <see langword="单开"/>
    /// </summary>
    private IEnumerator _MessageHandler()
    {
        while (true)
        {
            if (_messageHandlers.Count > 0)
            {
                MessageHandler handler = _messageHandlers.Dequeue();
                yield return handler.Send();
            }
            yield return null;
        }
    }

    /// <summary>
    /// 执行回调 <see langword="单开"/>
    /// </summary>
    private IEnumerator _CallBackHandler()
    {
        while (true)
        {
            if (_callBackHandlers.Count > 0)
            {
                CallBackHandler handler = _callBackHandlers.Dequeue();
                handler.Execute();
            }
            yield return null;
        }
    }

    /// <summary>
    /// 接受客户端消息 <see langword="一个客户端对应一个, 多开, 该协程应该被管理"/>
    /// </summary>
    private IEnumerator _Receive(NetworkPlayer networkPlayer)
    {
        //不为空则使用心跳包机制
        CheckHeartbeatEvent?.Invoke(networkPlayer);

        //客户端掉线则停止协程
        while (networkPlayer.Online)
        {
            //获取网络通讯流
            NetworkStream stream = networkPlayer.Stream;

            int length = 0;
            int receive = 0;
            //获取包头
            byte[] head = new byte[MsgConverter.MSG_HEAD];
            //异步读取
            IAsyncResult async = stream.BeginRead(head, 0, head.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            try
            {
                receive = stream.EndRead(async);

                if (receive < head.Length)
                {
                    networkPlayer.Close();
                    Debug.Log("消息包头接收失败");
                    yield break;
                }
            }
            catch (Exception ex)
            {
                networkPlayer.Close();
                Debug.Log("消息包头接收失败" + ex.Message);
                yield break;
            }

            //读取包体
            length = MsgConverter.GetHead(head);
            byte[] body = new byte[length - MsgConverter.MSG_HEAD];
            //异步读取
            async = stream.BeginRead(body, 0, body.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            //异常处理
            try
            {
                receive = stream.EndRead(async);
                if (receive < body.Length)
                {
                    networkPlayer.Close();
                    Debug.Log("消息包头接收失败");
                    yield break;
                }
            }
            catch (Exception ex)
            {
                networkPlayer.Close();
                Debug.Log("消息包体接收失败:" + ex.Message);
                yield break;
            }

            byte[] data;
            uint timeStamp;
            Msg msg = MsgConverter.UnPack(body, out timeStamp, out data);
            //赋值玩家最后一次发送消息时间
            networkPlayer.LastSendTime = TimeConverter.UInt2Float(timeStamp);

            //存入回调队列
            if (_callBacks.ContainsKey(msg))
            {
                _callBackHandlers.Enqueue(new CallBackHandler(networkPlayer, _callBacks[msg], data));
            }
            else
            {
                Debug.Log("服务器未注册该类型的回调事件");
            }
        }

        Debug.Log("一个客户端已掉线");
    }

    private void OnApplicationQuit()
    {
        ServerOfflineEvent?.Invoke();
    }
}