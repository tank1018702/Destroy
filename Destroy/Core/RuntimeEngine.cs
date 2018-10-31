using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Destroy
{
    public class RuntimeEngine
    {
        private readonly int tickPerSecond;
        private readonly bool block;
        private readonly List<GameObject> gameObjects;

        public static Action<GameObject> NewGameObject;

        public RuntimeEngine(int tickPerSecond, bool block)
        {
            this.tickPerSecond = tickPerSecond;
            this.block = block;
            gameObjects = new List<GameObject>();

            NewGameObject += gameObject =>
            {
                gameObjects.Add(gameObject);
                //注入gameObjects的引用
                FieldInfo gos = gameObject.GetType().GetField("gameObjects", BindingFlags.NonPublic | BindingFlags.Instance);
                gos.SetValue(gameObject, gameObjects);
            };

            CreatGameObjects();
        }

        public void Run()
        {
            Thread tick = new Thread
            (
                () =>
                {
                    float deltaTime = 0;

                    while (true)
                    {
                        int delayTime = 1000 / tickPerSecond;

                        InvokeScript(deltaTime);
                        //TODO


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
                CreatGameObject creatGameObject = _class.GetCustomAttribute<CreatGameObject>();
                //是否继承Script并且创建游戏物体
                if (_class.IsSubclassOf(typeof(Script)) && creatGameObject != null)
                {
                    OrderClass orderClass = new OrderClass(creatGameObject.CreatOrder, _class);

                    orderClasses.Add(orderClass);
                }
            }

            //Sorting(order越小的越先调用)
            foreach (var orderClass in InsertionSort(orderClasses))
            {
                CreatGameObject creatGameObject = orderClass.Type.GetCustomAttribute<CreatGameObject>();
                //设置GameObject与组件
                GameObject gameObject = new GameObject(creatGameObject.Name);

                //创建脚本实例
                object scriptInstance = assembly.CreateInstance($"{orderClass.Type.Namespace}.{orderClass.Type.Name}");
                //添加脚本实例作为组件
                gameObject.AddComponent((Component)scriptInstance);
                //add required components
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

        /// <summary>
        /// It has Bugs!
        /// </summary>
        private void InvokeScript(float deltaTime)
        {
            //统一调用Start
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];

                //反射获取components引用
                FieldInfo fieldInfo = gameObject.GetType().GetField("components", BindingFlags.NonPublic | BindingFlags.Instance);
                List<Component> components = (List<Component>)fieldInfo.GetValue(gameObject);

                for (int j = 0; j < components.Count; j++)
                {
                    //如果游戏物体被销毁则停止执行后续Start
                    if (!gameObjects.Contains(gameObject))
                        break;

                    Component component = components[j];
                    //筛选继承Script的组件
                    if (!component.GetType().IsSubclassOf(typeof(Script)))
                        continue;
                    Script script = (Script)component;
                    if (!script.Started)
                    {
                        script.Start();
                        script.Started = true;
                    }
                }
            }

            //统一调用Update
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];

                List<Script> scripts = gameObject.GetComponents<Script>();

                foreach (var script in scripts)
                {
                    //如果游戏物体被销毁则停止执行后续Start
                    if (!gameObjects.Contains(gameObject))
                        break;

                    //RemoveComponent加上判断, 如果在调用方法中移除其他脚本则跳出执行
                    if (!gameObject.HasComponent(script))
                        break;

                    script.Update(deltaTime); //在Update中new的Go会在下次生命周期开始时调用
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