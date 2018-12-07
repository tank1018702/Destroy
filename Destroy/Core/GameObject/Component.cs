namespace Destroy
{
    using System.Collections.Generic;

    public abstract class Component : Object
    {
        /// <summary>
        /// 游戏物体
        /// </summary>
        public GameObject gameObject { get; internal set; }

        /// <summary>
        /// Transform组件
        /// </summary>
        public Transform transform { get; internal set; }

        /// <summary>
        /// 添加指定组件
        /// </summary>
        public T AddComponent<T>() where T : Component, new() => gameObject.AddComponent<T>();

        /// <summary>
        /// 获取指定的类型及其子类(允许获取抽象类型子类)
        /// </summary>
        public T GetComponent<T>() where T : Component => gameObject.GetComponent<T>();

        /// <summary>
        /// 获取所有指定的类型及其子类(允许获取抽象类型子类)
        /// </summary>
        public List<T> GetComponents<T>() where T : Component => gameObject.GetComponents<T>();

        /// <summary>
        /// 获取组件个数
        /// </summary>
        public int ComponentCount => gameObject.ComponentCount;
    }
}