using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine : MonoBehaviour
{
    public static Coroutine Instance;

    private void Awake() => Instance = this;

    public void RunCoroutine(IEnumerator enumerator) => StartCoroutine(enumerator);
}

public class NetPlayer
{
    public TcpClient Client;
    public NetworkStream Stream;

    public NetPlayer(TcpClient client, NetworkStream stream)
    {
        this.Client = client;
        this.Stream = stream;
    }

    public void Send(byte[] data)
    {
        Coroutine.Instance.RunCoroutine(_Send(data));
    }

    IEnumerator _Send(byte[] data)
    {
        IAsyncResult async = Stream.BeginWrite(data, 0, data.Length, null, null);
        while (!async.IsCompleted)
            yield return null;

        Stream.EndWrite(async);
    }

    public void Receive()
    {

    }
}

public class Server : MonoBehaviour
{
    public string Address;
    public int Port;

    private TcpListener server;
    private List<NetPlayer> players;

    //帧同步关键数据
    private int frame;
    private int time;
    private bool ready;
    private List<bool> allReceived;

    void Awake()
    {
        GameObject obj = Instantiate(new GameObject("Coroutine"));
        obj.AddComponent<Coroutine>();

        server = new TcpListener(IPAddress.Parse(Address), Port);
        players = new List<NetPlayer>();
        frame = 0;
        time = 0;
        ready = false;
        allReceived = new List<bool>();

        server.Start(10); //开始监听
        Debug.Log("开启服务器");
        StartCoroutine(Accept());
    }

    private void FixedUpdate()
    {
        //人数已满
        if (players.Count == 2 && !ready)
        {
            foreach (var each in players)
            {
                byte[] data = new byte[2];
                data[0] = (byte)frame;
                data[1] = 1;
                each.Send(data);
            }
            ready = true;
        }

        if (!ready) return;

        if (time == 3) //一秒执行16次
        {
            time = 0;
            //收到所有信息
            if (allReceived.Count != players.Count)
            {
                Debug.Log("没有收到全部的包, 暂停一帧");
                return;
            }

            allReceived.Clear();
            Debug.Log("已经过了一网络帧, 向全体人员广播一次。");


            //遍历每个玩家
            foreach (var each in players)
            {
                byte[] data = new byte[2];
                data[0] = (byte)frame;
                data[1] = 1;
                each.Send(data);
            }
        }
        time++;
    }

    IEnumerator Accept()
    {
        while (true)
        {
            IAsyncResult async = server.BeginAcceptTcpClient(null, null);
            while (!async.IsCompleted)
                yield return null;

            TcpClient client = server.EndAcceptTcpClient(async);
            NetworkStream stream = client.GetStream();

            //构造网络玩家
            NetPlayer netPlayer = new NetPlayer(client, stream);
            players.Add(netPlayer);
            //接受消息
            StartCoroutine(Receive(netPlayer));
        }
    }

    /// <summary>
    /// 接受
    /// </summary>
    IEnumerator Receive(NetPlayer netPlayer)
    {
        while (true)
        {
            byte[] head = new byte[1];
            IAsyncResult async = netPlayer.Stream.BeginRead(head, 0, head.Length, null, null);
            while (!async.IsCompleted)
                yield return null;

            netPlayer.Stream.EndRead(async);
            //接受一个就增加一个
            allReceived.Add(true);
        }
    }
}