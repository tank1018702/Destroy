using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 挂在场景中的VStar Editor
/// </summary>
public sealed class VStarEditor : MonoBehaviour
{
    //模板
    [SerializeField]
    private List<Template> _pools = new List<Template>();

    private void Awake()
    {
        VStar.InitAll(_pools);
    }
}