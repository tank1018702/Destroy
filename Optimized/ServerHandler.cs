using UnityEngine;
using NetworkTools;
using PBMessage;
using Message;
using System.Collections;

public class ServerHandler : SceneTon<ServerHandler>
{
    public IPAddressHelper IP;                      //IP地址, 端口号设置
    [SerializeField]
    private HeartbeatSetting _heartbeatSetting;     //心跳包设置

    #region 心跳包机制

    private void _HeartBeatResponse(NetworkPlayer networkPlayer, byte[] data)
    {
        Debug.Log("接收到客户端发送的心跳包请求");
        networkPlayer.ReceivedHeartbeat = true;
        NetworkServer.Instance.Send(networkPlayer, new Msg(ActType.Response, CmdType.HeartBeat));
    }

    private IEnumerator _CheckHeartbeat(NetworkPlayer networkPlayer)
    {
        //针对单个客户端心跳包计时器
        float timer = 0;

        while (networkPlayer.Online)
        {
            timer += Time.fixedDeltaTime;

            //客户端连接后, 每隔k秒检测一次
            if (timer >= _heartbeatSetting.DelayTime)
            {
                if (!networkPlayer.ReceivedHeartbeat) //客户端连接后未发送心跳包, 移除客户端
                {
                    networkPlayer.Close();
                    Debug.Log("未接受到客户端心跳包回应, 客户端掉线");
                    yield break;
                }
                timer = 0;
                networkPlayer.ReceivedHeartbeat = false;
            }
            yield return null;
        }
    }

    #endregion

    private void _SpawnResponse(NetworkPlayer networkPlayer, byte[] data)
    {
        Debug.Log("接受到客户端发送的出生请求");
        NetworkServer.Instance.Send(networkPlayer, new Msg(ActType.Response, CmdType.Spawn), data);
    }

    private void _MoveResponse(NetworkPlayer networkPlayer, byte[] data)
    {
        NetworkServer.Instance.Enqueue(networkPlayer, new Msg(ActType.Response, CmdType.Move), data);
    }

    private void _OnlineResponse(NetworkPlayer networkPlayer, byte[] data)
    {
        ////客户端请求
        //OnlineRequest request = PBConverter.Deserializer<OnlineRequest>(data);

        //if (!NetworkServer.Instance.NetworkPlayersDict.ContainsKey(request.NetworkId))
        //{
        //    Debug.Log("检测到一个错误网络id玩家发送的信息");
        //    return;
        //}


        //匹配账号
        //PlayerAccount account = new PlayerAccount();
        //account.Account = request.Account;
        //account.Password = request.Password;

        //服务器回应
        //OnlineResponse response = new OnlineResponse();
        ////判断是否在配置账号中
        //if (StorageManager.Instance.CheckAccount(account))
        //{
        //    //登陆成功
        //    response.Suc = true;
        //}
        //else
        //{
        //    response.Suc = false;
        //}

        ////发送回应消息
        //byte[] sendData = PBConverter.Serializer(response);
        //NetworkServer.Instance.Send(networkPlayer, new Msg(ActType.Response, CmdType.Online), sendData);
    }

    private void _EditInfoResponse(NetworkPlayer networkPlayer, byte[] data)
    {
        EditInfoRequest request = PBConverter.Deserializer<EditInfoRequest>(data);

        if (!NetworkServer.Instance.NetworkPlayersDict.ContainsKey(request.NetworkId))
        {
            Debug.Log("检测到一个错误网络id玩家发送的信息");
            return;
        }

        //服务器回应
        bool suc = true;
        EditInfoResponse response = new EditInfoResponse();
        foreach (var each in NetworkServer.Instance.NetworkPlayersDict.Values)
        {
            //不允许重复名字
            if (each.Name == request.PlayerName)
            {
                suc = false;
                break;
            }
        }

        if (suc)
        {
            //给服务器玩家字典赋值
            NetworkServer.Instance.NetworkPlayersDict[request.NetworkId].Name = request.PlayerName;
        }
        response.Suc = suc;
        response.PlayerName = request.PlayerName;

        //发送回应消息
        byte[] sendData = PBConverter.Serializer(response);
        NetworkServer.Instance.Send(networkPlayer, new Msg(ActType.Response, CmdType.EditInfo), sendData);
    }

    private void Start()
    {
        //使用心跳包验证
        if (_heartbeatSetting.Use)
        {
            NetworkServer.Instance.Register(new Msg(ActType.Request, CmdType.HeartBeat), _HeartBeatResponse);
            //开启心跳包监听协程
            NetworkServer.Instance.CheckHeartbeatEvent += networkPlayer => StartCoroutine(_CheckHeartbeat(networkPlayer));
        }

        //玩家上线回调
        NetworkServer.Instance.Register(new Msg(ActType.Request, CmdType.Online), _OnlineResponse);

        //玩家编辑个人信息回调
        NetworkServer.Instance.Register(new Msg(ActType.Request, CmdType.EditInfo), _EditInfoResponse);

        NetworkServer.Instance.Register(new Msg(ActType.BroadCast, CmdType.Spawn), _SpawnResponse);
        NetworkServer.Instance.Register(new Msg(ActType.BroadCast, CmdType.Move), _MoveResponse);

        //创建服务器
        if (IP.Check(ref IP.Address, ref IP.Port))
        {
            NetworkServer.Instance.Connect(IP.Address, int.Parse(IP.Port));
        }
    }
}