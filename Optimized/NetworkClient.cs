using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Message;
using System.Net;

/// <summary>
/// 回调委托
/// </summary>
public delegate void ClientCallBack(byte[] data);

/// <summary>
/// 网络客户端
/// </summary>
public class NetworkClient : SingleTon<NetworkClient>
{
    public Action<string> ClientOfflineEvent;               //连接失败事件

    private Dictionary<Msg, ClientCallBack> _callBacks;     //回调字典

    private Queue<byte[]> _messages;                        //待发送消息队列
    private NetworkState _curState;                         //当前状态

    private TcpClient _client;                              //向服务器建立TCP连接并获取网络通讯流
    private NetworkStream _stream;                          //在网络通讯流中读写数据

    public float TimeLine { get; private set; }             //客户端已连接时间
    public uint TimeStamp                                   //时间戳
    {
        get { return TimeConverter.Float2UInt(TimeLine); }
    }

    private float LastReceiveMsgTime;                       //最后一次从服务器接受消息的时间

    /// <summary>
    /// 注册消息回调事件
    /// </summary>
    public void Register(Msg msg, ClientCallBack method)
    {
        if (method == null)
        {
            Debug.LogWarning("注册了空的回调方法");
            return;
        }
        if (_callBacks == null)
            _callBacks = new Dictionary<Msg, ClientCallBack>();

        if (!_callBacks.ContainsKey(msg))
            _callBacks.Add(msg, method);
        else
            Debug.LogWarning("注册了相同的回调事件");
    }

    /// <summary>
    /// 是否连接上服务器
    /// </summary>
    public bool Connected
    {
        get { return _curState == NetworkState.Connected; }
    }

    /// <summary>
    /// 连接服务器 <see langword="协程"/>
    /// </summary>
    public IEnumerator Connect(string address, int port)
    {
        //连接上后不能重复连接
        if (Connected)
        {
            Debug.Log("已经连接上服务器");
            yield break;
        }

        //初始化客户端
        _client = new TcpClient();
        
        //异步连接
        IAsyncResult async = _client.BeginConnect(IPAddress.Parse(address), port, null, null);
        while (!async.IsCompleted)
        {
            Debug.Log("连接服务器中");
            yield return null;
        }
        //异常处理
        try
        {
            _client.EndConnect(async);
            _stream = _client.GetStream();
        }
        catch (Exception ex)
        {
            Debug.Log("连接服务器失败:" + ex.Message);
            yield break;
        }

        if (_stream == null)
        {
            Debug.Log("连接服务器失败:数据流为空");
            yield break;
        }

        _curState = NetworkState.Connected;
        _messages = new Queue<byte[]>();

        Debug.Log("连接服务器成功");

        //注册客户端掉线事件
        ClientOfflineEvent += result =>
        {
            if (_client != null && _stream != null)
            {
                _stream?.Close();
                _stream?.Dispose();
                _client?.Close();
                _client?.Dispose();
            }
            _curState = NetworkState.None;
            Debug.LogWarning("客户端离线:" + result);
        };

        //开启计时器
        StartCoroutine(Timer());

        //设置异步发送消息
        StartCoroutine(_Send());
        //设置异步接收消息
        StartCoroutine(_Receive());
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public IEnumerator Timer()
    {
        TimeLine = Time.time;
        while (Connected)
        {
            TimeLine += Time.fixedDeltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 加入消息队列等待发送 <see langword="低优先级"/>
    /// </summary>
    public void Enqueue(Msg msg, byte[] data = null)
    {
        //加入待发送队列
        byte[] bytes = MsgConverter.Pack(msg, TimeStamp, data);
        _messages.Enqueue(bytes);
    }

    /// <summary>
    /// 开启协程发送消息 <see langword="高优先级"/>
    /// </summary>
    public void Send(Msg msg, byte[] data = null)
    {
        byte[] packet = MsgConverter.Pack(msg, TimeStamp, data);
        StartCoroutine(_Write(packet));
    }

    /// <summary>
    /// 在网络通讯流中写入信息 <see langword="多开, 有生命周期"/>
    /// </summary>
    private IEnumerator _Write(byte[] packet)
    {
        //如果服务器下线, 客户端依然会继续发消息
        if (!Connected || _stream == null)
        {
            Debug.Log("连接失败,无法发送消息");
            yield break;
        }

        //异步发送消息
        IAsyncResult async = _stream.BeginWrite(packet, 0, packet.Length, null, null);
        while (!async.IsCompleted)
        {
            yield return null;
        }
        //异常处理
        try
        {
            _stream.EndWrite(async);
        }
        catch (Exception ex)
        {
            ClientOfflineEvent("发送消息失败: " + ex.Message);
        }
    }

    /// <summary>
    /// 向服务器发送消息 <see langword="单开"/>
    /// </summary>
    private IEnumerator _Send()
    {
        //持续发送消息
        while (Connected)
        {
            //有待发送消息
            if (_messages.Count > 0)
            {
                byte[] data = _messages.Dequeue();
                yield return _Write(data);
            }
            yield return null;
        }
    }

    /// <summary>
    /// 从服务器接受消息 <see langword="单开"/>
    /// </summary>
    private IEnumerator _Receive()
    {
        //持续接受消息
        while (Connected)
        {
            int length = 0;
            int receive = 0;
            //获取包头
            byte[] head = new byte[MsgConverter.MSG_HEAD];
            //异步读取
            IAsyncResult async = _stream.BeginRead(head, 0, head.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            //异常处理
            try
            {
                receive = _stream.EndRead(async);

                if (receive < head.Length)
                {
                    ClientOfflineEvent("消息包头接收失败");
                    yield break;
                }
            }
            catch (Exception ex)
            {
                ClientOfflineEvent("消息包头接收失败:" + ex.Message);
                yield break;
            }

            //读取包体(总长度-包头2字节长度)
            length = MsgConverter.GetHead(head);
            byte[] body = new byte[length - MsgConverter.MSG_HEAD];
            //异步读取
            async = _stream.BeginRead(body, 0, body.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            //异常处理
            try
            {
                receive = _stream.EndRead(async);
                if (receive < body.Length)
                {
                    ClientOfflineEvent("消息包体接收失败");
                    yield break;
                }
            }
            catch (Exception ex)
            {
                ClientOfflineEvent("消息包体接收失败:" + ex.Message);
                yield break;
            }

            byte[] data;
            uint timeStamp;
            Msg msg = MsgConverter.UnPack(body, out timeStamp, out data);
            //记录上次接受到服务器消息的时间
            LastReceiveMsgTime = TimeConverter.UInt2Float(timeStamp);

            //执行回调事件
            if (_callBacks.ContainsKey(msg))
            {
                _callBacks[msg](data);
            }
            else
            {
                Debug.Log("客户端未注册该类型的回调事件");
            }
        }
    }

    private void OnApplicationQuit()
    {
        //事件不为空
        ClientOfflineEvent?.Invoke("客户端程序退出");
    }
}