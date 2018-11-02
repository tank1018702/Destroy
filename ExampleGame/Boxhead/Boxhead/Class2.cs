//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using Destroy;
//using Destroy.Net;

//public class Player
//{
//    public int Id;

//    private Socket socket;

//    public Player(int id, Socket socket)
//    {
//        Id = id;
//        this.socket = socket;
//    }

//    public byte[] HeadData { get; private set; }
//    private IAsyncResult headAsync;
//    public bool TryReceiveHead(int length)
//    {
//        if (headAsync != null && !headAsync.IsCompleted) //正在接受
//            return false;
//        if (headAsync != null && headAsync.IsCompleted)  //接受完成
//        {
//            socket.EndReceive(headAsync);
//            headAsync = null;
//            return true;
//        }
//        HeadData = new byte[length];
//        headAsync = socket.BeginReceive(HeadData, 0, HeadData.Length, SocketFlags.None, null, null);

//        if (headAsync.IsCompleted) //如果立马完成就直接返回
//        {
//            socket.EndReceive(headAsync);
//            headAsync = null;
//            return true;
//        }
//        else
//            return false;
//    }

//    public byte[] BodyData { get; private set; }
//    private IAsyncResult bodyAsync;
//    public bool TryReceiveBody(int length)
//    {
//        if (bodyAsync != null && !bodyAsync.IsCompleted) //正在收包
//            return false;
//        if (bodyAsync != null && bodyAsync.IsCompleted)  //收到包
//        {
//            socket.EndReceive(bodyAsync);
//            bodyAsync = null;
//            return true;
//        }

//        BodyData = new byte[length];
//        bodyAsync = socket.BeginReceive(BodyData, 0, BodyData.Length, SocketFlags.None, null, null);

//        if (bodyAsync.IsCompleted) //如果立马完成就直接返回
//        {
//            socket.EndReceive(bodyAsync);
//            bodyAsync = null;
//            return true;
//        }
//        else
//            return false;
//    }

//    public void Send(byte[] data) => socket.Send(data);
//}

//[CreatGameObject(1, "Server")]
//public class Server : Script
//{
//    private object locker = new object();

//    private int frameIndex;                                                      //游戏帧
//    private int playerId;                                                        //玩家自增id
//    private List<Player> players;                                                //玩家
//    private Dictionary<Player, Dictionary<int, C2S_InputMsg>> playerFrameInputs; //玩家与他某一帧的操作的操作的集合

//    public override void Start()
//    {
//        frameIndex = 0;
//        playerId = 0;
//        players = new List<Player>();
//        playerFrameInputs = new Dictionary<Player, Dictionary<int, C2S_InputMsg>>();

//        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//        EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
//        serverSocket.Bind(serverEP);
//        serverSocket.Listen(0);

//        Thread accept = new Thread(Accept) { IsBackground = true };
//        accept.Start(serverSocket);

//        Console.WriteLine("服务器开启成功!");
//    }

//    void Accept(object param)
//    {
//        Socket serverSocket = (Socket)param;

//        while (true)
//        {
//            Socket clientSocket = serverSocket.Accept();
//            Player player = new Player(playerId++, clientSocket);

//            lock (locker)
//            {
//                players.Add(player);
//            }

//            if (players.Count == 2)
//            {
//                Console.WriteLine("玩家人数已满, 开始游戏!");
//                StartGameBroadcast();

//                Thread receive = new Thread(Receive) { IsBackground = true };
//                Thread send = new Thread(Send) { IsBackground = true };
//                receive.Start();
//                send.Start();
//            }
//        }
//    }

//    void StartGameBroadcast()
//    {
//        foreach (var player in players)
//        {
//            S2C_StartGameMsg startGame = new S2C_StartGameMsg { id = player.Id };
//            byte[] data = NetworkUtils.Serialize(startGame);
//            player.Send(data);
//        }
//    }

//    void Receive()
//    {
//        while (true)
//        {
//            foreach (var player in players)
//            {
//                bool receiveHead = player.TryReceiveHead(4);
//                if (!receiveHead)
//                    continue;
//                int bodyLen = BitConverter.ToInt32(player.HeadData, 0);

//                bool receiveBody = player.TryReceiveBody(bodyLen);
//                if (!receiveBody)
//                    continue;

//                var input = NetworkUtils.Deserialize<C2S_InputMsg>(player.BodyData);

//                //更新该玩家的帧输入
//                Dictionary<int, C2S_InputMsg> frameInput = new Dictionary<int, C2S_InputMsg>();
//                frameInput.Add(input.frameIndex, input);
//                playerFrameInputs.Add(player, frameInput);
//            }
//            Thread.Sleep(10);
//        }
//    }

//    void Send()
//    {
//        while (true)
//        {
//            FrameBroadcast();
//            frameIndex++;
//            Thread.Sleep(50); // 20 times per second
//        }
//    }

//    void FrameBroadcast()
//    {
//        foreach (var player in players)
//        {
//            byte[] data1 = BitConverter.GetBytes(frameIndex);
//            byte[] data2 = NetworkUtils.Serialize(playerFrameInputs);
//            byte[] head = BitConverter.GetBytes(data2.Length);
//            List<byte> list = new List<byte>();
//            list.AddRange(head);    //4bytes
//            list.AddRange(data1);   //nbytes
//            list.AddRange(data2);   //4bytes

//            player.Send(list.ToArray());
//        }
//    }
//}