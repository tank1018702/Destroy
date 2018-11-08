using System;
using UnityEngine;
using NetworkTools;

/// <summary>
/// 网络状态
/// </summary>
public enum NetworkState
{
    None,      //未连接
    Connected, //已连接
}

/// <summary>
/// 场景单例 <see langword="必须挂载场景中"/>
/// </summary>
public class SceneTon<T> : MonoBehaviour where T : class
{
    protected SceneTon() { }

    protected virtual void Init() { }

    public static T Instance;

    private void Awake()
    {
        this.Init();
        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}

/// <summary>
/// 泛型单例 <see langword="不能挂载场景中"/>
/// </summary>
public class SingleTon<T> : MonoBehaviour where T : Component
{
    protected SingleTon() { }

    private static T _instance;

    //初始化T类型的对象(只有一次)
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                _instance = obj.AddComponent<T>();
                _instance.name = _instance.GetType().Name;
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
}

/// <summary>
/// IP地址助手
/// </summary>
[Serializable]
public class IPAddressHelper
{
    [HideInInspector]
    public string Address = null;
    [HideInInspector]
    public string Port = null;

    [SerializeField]
    private bool _useLocalIPv4;
    [SerializeField]
    private bool _use4399Port;

    public bool Check(ref string address, ref string portStr)
    {
        //如果使用本地IP
        if (_useLocalIPv4)
            address = NetworkUtils.GetLocalIPv4();
        //如果使用指定端口
        if (_use4399Port)
            portStr = "4399";

        //如果IP有问题
        if (!NetworkUtils.CheckIpv4String(address))
            return false;
        //端口有问题
        if (!NetworkUtils.CheckPortString(portStr))
            return false;

        return true;
    }

    /// <summary>
    /// <see langword="static"/>
    /// </summary>
    public static bool Check_S(string address, string portStr)
    {
        //如果IP有问题
        if (!NetworkUtils.CheckIpv4String(address))
            return false;
        //端口有问题
        if (!NetworkUtils.CheckPortString(portStr))
            return false;

        return true;
    }
}

/// <summary>
/// 心跳包设置
/// </summary>
[Serializable]
public class HeartbeatSetting
{
    public bool Use;

    public float DelayTime;
}