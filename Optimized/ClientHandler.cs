using System.Collections;
using UnityEngine;
using NetworkTools;
using Message;
using PBMessage;
using System;
using System.Collections.Generic;

public class ClientPlayer
{
    public int NetworkId;   //网络Id
    public bool Online;     //是否在线
    public string Name;     //名字
}

/// <summary>
/// 回调行为队列
/// </summary>
public class CallBackQueue
{
    private bool _stop;

    private List<ClientCallBack> _behaviors;

    public CallBackQueue()
    {
        _stop = false;
        _behaviors = new List<ClientCallBack>();
    }

    public void Add(ClientCallBack behavior)
    {
        //不能存在空的行为队列
        if (behavior != null)
            _behaviors.Add(behavior);
    }

    public void Stop() => _stop = true;

    public void Do(byte[] data)
    {
        _stop = false;
        foreach (var each in _behaviors)
        {
            if (_stop)
                break;
            try
            {
                each(data);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Stop();
            }
        }
    }
}

public class ClientHandler : SceneTon<ClientHandler>
{
    [SerializeField]
    private HeartbeatSetting _heartbeatSetting;                       //心跳包设置
    
    #region 心跳包机制

    private bool _clientReceived = true;

    private void _HeartBeatCallBack(byte[] data)
    {
        _clientReceived = true;
        Debug.Log("接收到服务器发送的心跳包回应");
    }

    private IEnumerator _HeartBeatRequest()
    {
        //连接上服务器马上发一个心跳包
        while (NetworkClient.Instance.Connected && _heartbeatSetting.Use)
        {
            if (!_clientReceived) //发送后未收到服务器回应, 自己掉线
            {
                NetworkClient.Instance.ClientOfflineEvent("心跳包接受失败, 断开连接");
                yield break;
            }
            //发送
            NetworkClient.Instance.Send(new Msg(ActType.Request, CmdType.HeartBeat));
            _clientReceived = false;
            Debug.Log("已发送心跳包请求");
            //每隔延迟时间发一次
            yield return new WaitForSecondsRealtime(_heartbeatSetting.DelayTime);
        }
    }

    #endregion

    [HideInInspector]
    public ClientPlayer Player;                                       //当前客户端玩家

    [HideInInspector]
    public bool ConnectRequesting;                                   //正在在线请求

    public CallBackQueue OnlineCallBack;                              //上线回调

    //private void _SpawnCallBack(byte[] data)
    //{
    //    Spawn response = PBConverter.Deserializer<Spawn>(data);

    //    if (NetworkManager.Instance.PlayerManager == null)                  //字典必须初始化
    //        return;
    //    if (NetworkManager.Instance.PlayerManager.ContainsKey(response.Id)) //不能重复
    //        return;

    //    //生成该玩家
    //    GameObject instance = GameObject.Instantiate(NetworkManager.Instance.OtherPlayerPrefab);
    //    //同步Transform
    //    response.Transform.ToTransform(instance.transform);
    //    //添加进角色管理器中
    //    NetworkManager.Instance.PlayerManager.Add(response.Id, instance);

    //    Debug.Log("有一个玩家加入游戏");
    //}

    //private void _MoveCallBack(byte[] data)
    //{
    //    Move response = PBConverter.Deserializer<Move>(data);

    //    if (NetworkManager.Instance.PlayerManager == null)
    //        return;
    //    if (!NetworkManager.Instance.PlayerManager.ContainsKey(response.Id))
    //    {
    //        Debug.Log("BUG:玩家还没有创建出来, 无法移动");
    //        return;
    //    }

    //    //移动该玩家
    //    GameObject otherPlayer = NetworkManager.Instance.PlayerManager[response.Id];
    //    response.Transform.ToTransform(otherPlayer.transform);
    //}

    private void _ConnectCallBack(byte[] data)
    {
        ConnectResponse response = PBConverter.Deserializer<ConnectResponse>(data);
        //赋上服务器发送的网络id
        Player.NetworkId = response.NetworkId;
    }

    private void _OnlineCallBack(byte[] data)
    {
        //OnlineResponse response = PBConverter.Deserializer<OnlineResponse>(data);
        //if (response.Suc)
        //{
        //    Player.Online = true; //成功上线
        //    Debug.Log($"登陆成功:{Player.Online}");
        //}
        //else
        //{
        //    OnlineCallBack.Stop(); //停止接下来的回调操作, 不执行场景跳转
        //    Debug.Log("登陆失败");
        //}
        //ConnectRequesting = false; //收到回应
    }

    private void _EditInfoCallBack(byte[] data)
    {
        EditInfoResponse response = new EditInfoResponse();

        if (response.Suc)
        {
            Player.Name = response.PlayerName;
            Debug.Log("编辑个人信息成功");
        }
        else
        {
            Debug.Log("编辑个人信息失败");
        }
    }

    private void OnEnable() //必须比UIManager.Start更先调用
    {
        //客户端初始化
        Player = new ClientPlayer();

        //注册连接服务器回调事件
        NetworkClient.Instance.Register(new Msg(ActType.Response, CmdType.Connect), _ConnectCallBack);

        //注册上线添加连锁事件
        OnlineCallBack = new CallBackQueue();
        OnlineCallBack.Add(_OnlineCallBack);
        ClientCallBack clientCallBack = data => this.OnlineCallBack.Do(data); //闭包调用
        NetworkClient.Instance.Register(new Msg(ActType.Response, CmdType.Online), clientCallBack);

        //注册编辑个人信息回调事件
        NetworkClient.Instance.Register(new Msg(ActType.Response, CmdType.EditInfo), _EditInfoCallBack);
    }

    /// <summary>
    /// 连接请求
    /// </summary>
    public IEnumerator ConnectRequest(string address, string portStr, string playerName)
    {
        //如果没有建立网络连接先建立网络连接
        if (!NetworkClient.Instance.Connected)
        {
            //连接
            yield return NetworkClient.Instance.Connect(address, int.Parse(portStr));

            //Check State
            if (!NetworkClient.Instance.Connected)
                yield break;
        }
        //发送连接请求
        ConnectRequest request = new ConnectRequest { PlayerName = playerName };
        byte[] data = PBConverter.Serializer(request);
        NetworkClient.Instance.Send(new Msg(ActType.Request, CmdType.Connect), data);

        this.ConnectRequesting = true; //正在请求消息

        //计时器等待服务器回应
        float timer = 0;
        while (true)
        {
            timer += Time.fixedDeltaTime;
            //收到回应
            if (!this.ConnectRequesting)
                yield break;
            //请求超时5S
            if (timer >= 5)
            {
                Debug.Log("请求超时.");
                this.ConnectRequesting = false;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 编辑个人信息请求
    /// </summary>
    public void EditInfoRequest(string playerName)
    {
        if (!Player.Online)
            return;

        EditInfoRequest request = new EditInfoRequest
        {
            NetworkId = Player.NetworkId,
            PlayerName = playerName
        };

        byte[] data = PBConverter.Serializer(request);
        NetworkClient.Instance.Send(new Msg(ActType.Request, CmdType.EditInfo), data);
    }
}