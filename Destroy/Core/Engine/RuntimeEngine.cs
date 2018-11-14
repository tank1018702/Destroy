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

        public RuntimeEngine()
        {
            //Initial
            gameObjects = new List<GameObject>();
            //Register Method
            NewGameObject += gameObject =>
            {
                gameObjects.Add(gameObject);
                //注入gameObjects的引用
                RuntimeReflector.SetPrivateField(gameObject, "gameObjects", gameObjects);
            };
        }

        public void Run(int tickPerSecond, bool allowMultiple = true)
        {
            //Singleton Check
            if (!allowMultiple)
            {
                Mutex mutex = new Mutex(true, Process.GetCurrentProcess().ProcessName, out bool nonexsitent);
                if (!nonexsitent)
                {
                    Debug.Error("该游戏不允许多个实例同时运行!");
                    Thread.Sleep(2500);
                    Environment.Exit(0);
                }
            }

            CreatScriptableGameObjects();

            Time time = new Time();
            float tickTime = (float)1 / tickPerSecond;
            int delayTime = 1000 / tickPerSecond;
            float deltaTime = 0;

            //设置Time类属性
            RuntimeReflector.SetStaticPrivateProperty(time, "TickTime", tickTime);

            Stopwatch stopwatch = new Stopwatch();

            while (true)
            {
                //开始计时
                stopwatch.Restart();
                //设置Time类属性
                RuntimeReflector.SetStaticPrivateProperty(time, "DeltaTime", deltaTime);

                #region LifeCycle

                CallScriptMethod(gameObjects, "Start", true);   //调用Start
                CollisionSystem.Update(gameObjects);            //碰撞检测
                CallScriptMethod(gameObjects, "Update", false); //调用Update
                RendererSystem.Update(gameObjects);             //渲染物体

                #endregion

                Thread.Sleep(delayTime);
                //计算时间
                deltaTime = stopwatch.ElapsedMilliseconds / (float)1000;
            }
        }

        private void CreatScriptableGameObjects()
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
            foreach (OrderClass orderClass in InsertionSort(orderClasses))
            {
                CreatGameObject creatGameObject = orderClass.Type.GetCustomAttribute<CreatGameObject>();
                //创建游戏物体
                GameObject gameObject = new GameObject(creatGameObject.Name);

                //添加脚本组件(脚本必须包含public无参构造方法, 并且这里会调用一次构造)
                gameObject.AddComponent(orderClass.Type);
                //添加required组件
                foreach (Type type in creatGameObject.RequiredComponents)
                {
                    //如果继承Component类型(必须包含public无参构造方法)
                    if (type.IsSubclassOf(typeof(Component)))
                        gameObject.AddComponent(type);
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

        public static void CallScriptMethod(List<GameObject> gameObjects, string methodName, bool start = false, params object[] parameters)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                //反射获取components引用实现动态遍历components
                List<Component> components = (List<Component>)RuntimeReflector.GetPrivateField(gameObject, "components");

                for (int j = 0; j < components.Count; j++)
                {
                    Component component = components[j];
                    //筛选继承Script的组件
                    if (!component.GetType().IsSubclassOf(typeof(Script)))
                        continue;
                    //如果游戏物体被销毁则停止执行后续Script
                    if (!gameObjects.Contains(gameObject))
                        break;
                    Script script = (Script)component;
                    //Call Once
                    if (start && script.Started)
                        return;
                    if (start)
                        script.Started = true;
                    RuntimeReflector.InvokePublicMethod(script, methodName, parameters);
                }
            }
        }

        public static void CallScriptMethod(GameObject gameObject, string methodName, bool start = false, params object[] parameters)
        {
            List<GameObject> gameObjects = new List<GameObject> { gameObject };
            CallScriptMethod(gameObjects, methodName, start, parameters);
        }
    }
}