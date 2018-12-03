using UnityEngine;
using System.Collections;
/// <summary>
/// 挂在物体上生命周期脚本
/// </summary>
public class LifeCycle : MonoBehaviour
{
    //生命时长
    public float Time;

    private void OnEnable()
    {
        StartCoroutine(_Disable());
    }

    private IEnumerator _Disable()
    {
        yield return new WaitForSeconds(Time);
        VStar.Set(gameObject);
    }
}