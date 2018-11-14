namespace Destroy
{
    using System;
    using System.Collections.Generic;

    public abstract class Component
    {
        /// <summary>
        /// 是否进行了初始化
        /// </summary>
        public bool Initialized;

        /// <summary>
        /// 游戏物体
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// Transform组件
        /// </summary>
        public Transform transform;

        /// <summary>
        /// 添加指定组件
        /// </summary>
        public T AddComponent<T>() where T : Component, new() => gameObject.AddComponent<T>();

        /// <summary>
        /// 添加指定组件
        /// </summary>
        public Component AddComponent(Type type) => gameObject.AddComponent(type);

        /// <summary>
        /// 获取指定的类型
        /// </summary>
        public T GetComponent<T>() where T : Component => gameObject.GetComponent<T>();

        /// <summary>
        /// 获取所有指定的类型及其子类
        /// </summary>
        public List<T> GetComponents<T>() where T : Component => gameObject.GetComponents<T>();

        /// <summary>
        /// 移除指定组件
        /// </summary>
        public void RemoveComponent<T>() where T : Component => gameObject.RemoveComponent<T>();

        /// <summary>
        /// 根据名字寻找场景中一个游戏物体, 若有多个同名物体也只返回一个。
        /// </summary>
        public GameObject Find(string name) => gameObject.Find(name);

        /// <summary>
        /// 销毁一个游戏物体, 不会立马销毁, 会等到调用该方法的方法执行结束后进行销毁处理
        /// </summary>
        public void Destroy(GameObject gameObject) => this.gameObject.Destroy(gameObject);

        /// <summary>
        /// 获取游戏物体个数
        /// </summary>
        public int GameObjectCount => gameObject.GameObjectCount;

        /// <summary>
        /// 获取组件个数
        /// </summary>
        public int ComponentCount => gameObject.ComponentCount;
    }
}