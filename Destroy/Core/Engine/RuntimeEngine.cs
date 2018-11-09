namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    public class RuntimeEngine
    {
        private readonly List<GameObject> gameObjects;

        public static Action<GameObject> NewGameObject;

        public RuntimeEngine(bool allowMultiple = true)
        {
            //Singleton Check
            if (!allowMultiple)
            {
                Mutex mutex = new Mutex(true, Process.GetCurrentProcess().ProcessName, out bool nonexsitent);
                if (!nonexsitent)
                {
                    Debug.Error("该游戏不允许多个实例同时运行!");
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
            }

            //Initial
            gameObjects = new List<GameObject>();
            NewGameObject += gameObject =>
            {
                gameObjects.Add(gameObject);
                //注入gameObjects的引用
                Reflector.SetPrivateField(gameObject, "gameObjects", gameObjects);
            };
        }

        public void Run(int tickPerSecond, bool block = true)
        {
            CreatGameObjects();

            Time time = new Time();
            float tickTime = (float)1 / tickPerSecond;
            int delayTime = 1000 / tickPerSecond;

            //设置Time类属性
            Reflector.SetStaticPrivateProperty(time, "TickTime", tickTime);

            Thread lifeCycle = new Thread
            (
                () =>
                {
                    //I use the simple fixed deltaTime but not use the System.Diagnostics.
                    float deltaTime = 0;
                    Stopwatch stopwatch = new Stopwatch();

                    while (true)
                    {
                        //开始计时
                        stopwatch.Restart();
                        //设置Time类属性
                        Reflector.SetStaticPrivateProperty(time, "DeltaTime", deltaTime);
                        //LifeCycle
                        ScriptSystem.InvokeScript(gameObjects); //运行脚本
                        RSystem.RenderGameObject(gameObjects);  //渲染
                        //etc.
                        Thread.Sleep(delayTime);
                        //计算时间
                        deltaTime = stopwatch.ElapsedMilliseconds / (float)1000;
                    }
                }
            )
            { IsBackground = !block };

            lifeCycle.Start();
        }

        private void CreatGameObjects()
        {
            Assembly assembly = Assembly.GetEntryAssembly(); //获取调用该方法的程序集而不是引擎所在的程序集

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

                //创建脚本实例(必须包含public无参构造方法, 并且这里会调用一次构造)
                object scriptInstance = assembly.CreateInstance($"{orderClass.Type.Namespace}.{orderClass.Type.Name}");
                //添加脚本实例作为组件
                gameObject.AddComponent((Component)scriptInstance);
                //add required components
                foreach (Type type in creatGameObject.RequiredComponents)
                {
                    //如果继承Component类型(必须为public无参构造方法)
                    if (type.IsSubclassOf(typeof(Component)))
                    {
                        object component = assembly.CreateInstance($"{type.Namespace}.{type.Name}");
                        gameObject.AddComponent((Component)component);
                    }
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