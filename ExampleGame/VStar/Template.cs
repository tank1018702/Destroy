using System;
using UnityEngine;
/// <summary>
/// 模板
/// </summary>
[Serializable]
public class Template
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public string Id;

    /// <summary>
    /// 游戏物体
    /// </summary>
    public GameObject Obj;

    /// <summary>
    /// 初始化大小
    /// </summary>
    public int Size;

    /// <summary>
    /// 生命时长
    /// </summary>
    public float LifeTime;

    /// <summary>
    /// 构造新模板
    /// </summary>
    /// <param name="id">唯一标识</param>
    /// <param name="obj">游戏物体</param>
    /// <param name="size">初始化大小</param>
    /// <param name="lifeTime">生命时长</param>
    public Template(string id, GameObject obj, int size, float lifeTime)
    {
        Id = id;
        Obj = obj;
        Size = size;
        LifeTime = lifeTime;
    }
}