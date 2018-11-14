﻿namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class GameObject
    {
        public string Name;

        private List<GameObject> gameObjects;

        private List<Component> components;

        /// <summary>
        /// 获取游戏物体个数
        /// </summary>
        public int GameObjectCount => gameObjects.Count;

        /// <summary>
        /// 获取组件个数
        /// </summary>
        public int ComponentCount => components.Count;

        /// <summary>
        /// 创建一个被Engine托管的新游戏物体
        /// </summary>
        public GameObject()
        {
            Name = "GameObject";
            components = new List<Component>();
            RuntimeEngine.NewGameObject(this);
        }

        /// <summary>
        /// 创建一个被Engine托管的新游戏物体
        /// </summary>
        public GameObject(string name)
        {
            Name = name;
            components = new List<Component>();
            RuntimeEngine.NewGameObject(this);
        }

        /// <summary>
        /// 添加指定组件
        /// </summary>
        public T AddComponent<T>() where T : Component, new()
        {
            foreach (var component in components)
                if (typeof(T) == component.GetType())
                    return null;
            T instance = new T { gameObject = this };
            components.Add(instance);

            return instance;
        }

        /// <summary>
        /// 添加指定组件
        /// </summary>
        public Component AddComponent(Type type)
        {
            foreach (var each in components)
                if (each.GetType() == type)
                    return null;
            Assembly assembly = Assembly.GetEntryAssembly();

            Component component = (Component)assembly.CreateInstance($"{type.Namespace}.{type.Name}");
            component.gameObject = this;
            components.Add(component);
            return component;
        }

        /// <summary>
        /// 获取指定的类型
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            foreach (var component in components)
            {
                //返回同类型或子类
                if (typeof(T) == component.GetType())
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
            foreach (var component in components)
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
        /// 移除指定组件
        /// </summary>
        public void RemoveComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                    components.RemoveAt(i);
            }
        }

        /// <summary>
        /// 根据名字寻找场景中一个游戏物体, 若有多个同名物体也只返回一个。
        /// </summary>
        public GameObject Find(string name)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.Name == name)
                    return gameObject;
            }
            return null;
        }

        /// <summary>
        /// 销毁一个游戏物体, 不会立马销毁, 会等到调用该方法的方法执行结束后进行销毁处理
        /// </summary>
        public void Destroy(GameObject gameObject) => gameObjects.Remove(gameObject);
    }
}