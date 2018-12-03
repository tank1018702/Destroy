using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// VStar1.1 By Charlie179 (2018/8/31)
/// </summary>
public static class VStar
{
    /// <summary>
    /// (回收站)模板ID, 对象池
    /// </summary>
    private static readonly Dictionary<string, List<GameObject>> _recycleBin = new Dictionary<string, List<GameObject>>();

    /// <summary>
    /// 初始化对象池大小
    /// </summary>
    private static void _InitSize(Template template)
    {
        List<GameObject> objects = new List<GameObject>();
        //创建
        for (int i = 0; i < template.Size; i++)
        {
            GameObject gameObject = Get(template.Id);
            objects.Add(gameObject);
        }
        //存入对象池
        for (int i = 0; i < objects.Count; i++)
        {
            Set(objects[i]);
        }
    }

    /// <summary>
    /// 判断是否存在与该ID相对应的对象池
    /// </summary>
    private static bool _Exist(string id)
    {
        if (_recycleBin.ContainsKey(id))
            return true;
        else
            return false;
    }

    #region 外部调用

    /// <summary>
    /// 初始化
    /// </summary>
    public static void InitAll(List<Template> templates)
    {
        //遍历模板列表
        templates.ForEach
        (
            (each) =>
            {
                if (each != null)
                {
                    //生成对象池成功
                    if (Init(each))
                    {
                        //初始化对象池大小
                        _InitSize(each);
                    }
                }
            }
        );

        //打印结果(Test Only)
        int count = 0;
        foreach (var each in _recycleBin.Keys)
        {
            if (each != null)
            {
                Debug.Log("生成一个对象池:" + each);
                count++;
            }
        }
    }

    /// <summary>
    /// 清空所有对象池
    /// </summary>
    public static void RemoveAll()
    {
        _recycleBin.Clear();
    }

    /// <summary>
    /// 根据模板添加对象池 <see langword="如果要删除该物体上的LifeCycle组件, 需要把LifeCycle的时间设置为0"/>
    /// </summary>
    public static bool Init(Template template)
    {
        if (template == null || !template.Obj)
        {
            Debug.LogWarning("对象池初始化:空引用异常");
            return false;
        }
        if (string.IsNullOrEmpty(template.Id))
        {
            Debug.LogWarning("创建失败:ID为空");
            return false;
        }
        if (_Exist(template.Id))
        {
            Debug.LogWarning("创建失败:已有相同ID");
            return false;
        }
        if (template.Size < 0)
        {
            Debug.LogWarning("初始化大小不能为负数");
            return false;
        }
        if (template.LifeTime < 0)
        {
            Debug.LogWarning("生命时长不能为负数");
            return false;
        }
        _recycleBin.Add(template.Id, new List<GameObject>());
        //设置第一个为模板
        var list = _recycleBin[template.Id];
        //设置生命时长
        if (template.LifeTime > 0)
        {
            LifeCycle time = template.Obj.GetComponent<LifeCycle>();
            if (time == null)
            {
                time = template.Obj.AddComponent<LifeCycle>();
            }
            time.Time = template.LifeTime;
        }
        //删除生命时长
        else if (template.LifeTime == 0)
        {
            LifeCycle time = template.Obj.GetComponent<LifeCycle>();
            if (time != null)
            {
                Object.DestroyImmediate(time, true); //DestroyImmediate方法把预支体都删除
            }
        }
        list.Add(template.Obj);
        return true;
    }

    /// <summary>
    /// 根据模板ID移除对象池
    /// </summary>
    public static bool Remove(string id)
    {
        if (!string.IsNullOrEmpty(id) && _Exist(id))
        {
            _recycleBin.Remove(id);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 根据ID拿到相应实例
    /// </summary>
    public static GameObject Get(string id)
    {
        if (!_Exist(id))
        {
            Debug.LogWarning("获取实例失败");
            return null;
        }
        var list = _recycleBin[id];
        //遍历ID对象池
        for (int i = 1; i < list.Count; i++)
        {
            //防止对象池中有空元素
            if (list[i] == null)
            {
                list.RemoveAt(i);
                continue;
            }
            //判断是否在场景中激活。
            if (list[i].activeInHierarchy == false)
            {
                //激活并返回对象
                list[i].SetActive(true);
                return list[i];
            }
        }
        GameObject obj = Object.Instantiate(list[0]);
        list.Add(obj);
        return obj;
    }

    /// <summary>
    /// 根据ID拿到相应实例
    /// </summary>
    /// <param name="id">物体Id</param>
    /// <param name="position">物体坐标</param>
    /// <returns></returns>
    public static GameObject Get(string id, Vector3 position)
    {
        GameObject go = Get(id);
        if (!go)
            return null;

        go.transform.position = position;
        return go;
    }

    /// <summary>
    /// 根据ID拿到相应实例
    /// </summary>
    /// <param name="id">物体Id</param>
    /// <param name="position">物体坐标</param>
    /// <param name="rotation">物体旋转</param>
    /// <returns></returns>
    public static GameObject Get(string id, Vector3 position, Quaternion rotation)
    {
        GameObject go = Get(id, position);
        if (!go)
            return null;

        go.transform.rotation = rotation;
        return go;
    }

    /// <summary>
    /// 根据ID拿到相应实例
    /// </summary>
    /// <param name="id">物体Id</param>
    /// <param name="position">物体坐标</param>
    /// <param name="rotation">物体旋转</param>
    /// <param name="parent">物体父物体</param>
    /// <returns></returns>
    public static GameObject Get(string id, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = Get(id, position, rotation);
        if (!go)
            return null;

        go.transform.parent = parent;
        return go;
    }

    /// <summary>
    /// 把实例存回对象池
    /// </summary>
    public static void Set(GameObject go)
    {
        if (!go)
        {
            Debug.LogWarning("实例为空");
            return;
        }
        go.SetActive(false);
    }

    #endregion
}