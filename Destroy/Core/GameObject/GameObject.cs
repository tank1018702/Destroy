namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// 使用该方法创建一个游戏物体
    /// </summary>
    public delegate GameObject Instantiate();

    public class GameObject : Object
    {
        internal List<Component> Components;

        /// <summary>
        /// 添加指定组件
        /// </summary>
        internal Component AddComponent(Type type)
        {
            foreach (var each in Components)
                if (each.GetType() == type)
                    return null;
            Assembly assembly = RuntimeEngine.GetAssembly;

            Component component = (Component)assembly.CreateInstance($"{type.Namespace}.{type.Name}");
            component.Name = type.Name;
            component.Active = true;
            component.gameObject = this;
            component.transform = transform;
            Components.Add(component);

            return component;
        }

        /// <summary>
        /// 获取指定组件
        /// </summary>
        internal Component GetComponent(Type type)
        {
            foreach (var each in Components)
                if (each.GetType() == type)
                    return each;
            return null;
        }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag;

        /// <summary>
        /// 游戏物体
        /// </summary>
        public GameObject gameObject { get; protected set; }

        /// <summary>
        /// Transform组件
        /// </summary>
        public Transform transform { get; protected set; }

        /// <summary>
        /// 创建一个游戏物体 (相当于在当前场景中实例化)
        /// </summary>
        public GameObject() => Init();

        /// <summary>
        /// 创建一个游戏物体 (相当于在当前场景中实例化)
        /// </summary>
        public GameObject(string name)
        {
            Init();
            Name = name;
        }

        /// <summary>
        /// 添加指定组件
        /// </summary>
        public T AddComponent<T>() where T : Component, new()
        {
            foreach (var component in Components)
                if (typeof(T) == component.GetType())
                    return null;

            T instance = new T();
            instance.Name = typeof(T).Name;
            instance.Active = true;
            instance.gameObject = this;
            instance.transform = transform;
            Components.Add(instance);

            return instance;
        }

        /// <summary>
        /// 获取指定的类型及其子类
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            Type t = typeof(T);
            foreach (var component in Components)
            {
                //返回同类型或子类
                if (component.GetType() == t || component.GetType().IsSubclassOf(t))
                    return component as T;
            }
            return null;
        }

        /// <summary>
        /// 获取所有指定的类型及其子类
        /// </summary>
        public List<T> GetComponents<T>() where T : Component
        {
            List<T> list = new List<T>();
            foreach (var component in Components)
            {
                var type = typeof(T);
                var comType = component.GetType();
                //返回同类型或子类
                if (type == comType || comType.IsSubclassOf(type))
                    list.Add(component as T);
            }
            return list;
        }

        /// <summary>
        /// 获取组件个数
        /// </summary>
        public int ComponentCount => Components.Count;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            Name = "GameObject";
            Active = true;
            Tag = "None";
            gameObject = this;
            Components = new List<Component>();
            transform = AddComponent<Transform>(); //添加默认组件
            transform.transform = transform;
            RuntimeEngine.Manage(this); //进入托管模式
        }

        /// <summary>
        /// 在当前场景中根据名字寻找游戏物体, 若有多个同名物体也只返回一个。
        /// </summary>
        public static GameObject Find(string name)
        {
            foreach (var gameObject in GameObjects)
            {
                if (gameObject.Name == name)
                    return gameObject;
            }
            return null;
        }

        /// <summary>
        /// 在当前场景中根据标签寻找游戏物体, 若有多个则返回多个。
        /// </summary>
        public static GameObject[] FindWithTag(string tag)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (var gameObject in GameObjects)
            {
                if (gameObject.Tag == tag)
                    list.Add(gameObject);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取当前场景中游戏物体的个数
        /// </summary>
        public static int Count => GameObjects.Count;
    }
}