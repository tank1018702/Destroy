using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Destroy
{
    public class RuntimeEngine
    {
        private int tickPerSecond;
        private bool block;
        private List<GameObject> gameObjects;

        public RuntimeEngine(int tickPerSecond, bool block)
        {
            this.tickPerSecond = tickPerSecond;
            this.block = block;
            gameObjects = new List<GameObject>();

            CreatGameObjects();
        }

        public void Start() => CallMethod("Start");

        public void Tick()
        {
            Thread tick = new Thread
            (
                () =>
                {
                    float deltaTime = 0;

                    while (true)
                    {
                        int delayTime = 1000 / tickPerSecond;

                        CallMethod("Update", deltaTime);
                        Thread.Sleep(delayTime);

                        deltaTime = (float)1 / tickPerSecond;
                    }
                }
            )
            {
                IsBackground = !block
            };

            tick.Start();
        }

        private void CreatGameObjects()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            List<OrderClass> orderClasses = new List<OrderClass>();

            //获取游戏物体上的脚本
            foreach (var _class in assembly.GetTypes())
            {
                //是否继承Script并且创建游戏物体
                if (_class.IsSubclassOf(typeof(Script)) && _class.GetCustomAttribute<CreatGameObject>() != null)
                {
                    OrderClass orderClass = new OrderClass(uint.MaxValue, _class);

                    //判断执行顺序
                    UpdateOrder updateOrder = _class.GetCustomAttribute<UpdateOrder>();
                    if (updateOrder != null)
                        orderClass.Order = updateOrder.Order;

                    orderClasses.Add(orderClass);
                }
            }
            //Sorting(order越小的越先调用)
            foreach (var orderClass in InsertionSort(orderClasses))
            {
                //创建脚本实例
                object scriptInstance = assembly.CreateInstance($"{orderClass.Type.Namespace}.{orderClass.Type.Name}");

                //设置GameObject与组件
                GameObject gameObject = new GameObject();
                //添加进有序游戏物体集合
                gameObjects.Add(gameObject);

                //添加脚本实例作为组件
                gameObject.AddComponent((Component)scriptInstance);
                //添加Required组件
                CreatGameObject creatGameObject = orderClass.Type.GetCustomAttribute<CreatGameObject>();
                foreach (var type in creatGameObject.RequiredComponents)
                {
                    //如果继承Component类型
                    if (type.IsSubclassOf(typeof(Component)))
                    {
                        object component = assembly.CreateInstance($"{type.Namespace}.{type.Name}");
                        gameObject.AddComponent((Component)component);
                    }
                }
            }
        }

        private void CallMethod(string methodName, params object[] parameters)
        {
            //遍历游戏物体
            foreach (var gameObject in gameObjects)
            {
                List<Script> scripts = gameObject.GetComponents<Script>();
                //调用每个脚本的指定方法
                foreach (var script in scripts)
                {
                    MethodInfo method = script.GetType().GetMethod(methodName);
                    method?.Invoke(script, parameters);
                }
            }
        }

        private class OrderClass
        {
            public uint Order;
            public Type Type;

            public OrderClass(uint order, Type type)
            {
                Order = order;
                Type = type;
            }

            public OrderClass Copy() => new OrderClass(Order, Type);
        }

        private List<OrderClass> InsertionSort(List<OrderClass> orderClasses)
        {
            int preIndex;
            OrderClass current;
            for (int i = 1; i < orderClasses.Count; i++)
            {
                preIndex = i - 1;
                current = orderClasses[i].Copy();

                while (preIndex >= 0 && orderClasses[preIndex].Order > current.Order)
                {
                    orderClasses[preIndex + 1] = orderClasses[preIndex]; //前一个覆盖当前
                    preIndex--;
                }
                orderClasses[preIndex + 1] = current;
            }
            return orderClasses;
        }
    }
}