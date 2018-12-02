namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using Destroy.Net;

    public class RuntimeEngine
    {
        public static Action<GameObject> Manage;
        public Thread GameThread { get; private set; }

        private readonly List<GameObject> gameObjects;
        private readonly RuntimeDebugger debugger;

        public RuntimeEngine(RuntimeDebugger debugger = null)
        {
            //Register Method
            Manage += gameObject => gameObjects.Add(gameObject);
            //Initial
            gameObjects = new List<GameObject>();
            this.debugger = debugger;
            GameThread = null;

            //创建静态GameObject
            GameObject tempGo = new GameObject();
            RuntimeReflector.SetPrivateStaticField(tempGo, "gameObjects", gameObjects);
            //创建静态Object
            Object tempObj = new Object();
            RuntimeReflector.SetPrivateStaticField(tempObj, "gameObjects", gameObjects);
            tempObj = null;
            Object.Destroy(tempGo);
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
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                }
            }

            GameThread = Thread.CurrentThread;
            CreateGameObjects();

            Stopwatch stopwatch = new Stopwatch();
            Time time = new Time();
            int tickTime = 1000 / tickPerSecond < 1 ? 1 : 1000 / tickPerSecond; //每帧因该花的时间(最少应为1毫秒)
            float deltaTime = 0; //这一帧距离上一帧的时间
            float totalTime = 0; //总执行时间

            while (true)
            {
                //开始计时
                stopwatch.Restart();
                //设置Time类属性
                RuntimeReflector.SetPublicStaticProperty(time, "DeltaTime", deltaTime);
                totalTime += deltaTime;
                RuntimeReflector.SetPublicStaticProperty(time, "TotalTime", totalTime);

                UpdateGameObjects();    //更新所有游戏物体
                if (debugger != null)   //更新调试器
                    debugger.Watch();

                //计算应该休眠的时间, 保证每秒运行相应Tick次数
                int runTime = (int)stopwatch.ElapsedMilliseconds;
                int delayTime = tickTime - runTime;
                if (delayTime > 0)
                {
                    deltaTime = (float)tickTime / 1000;
                    Thread.Sleep(delayTime); //不是高精度计时器, 只能起到模拟的效果
                }
                //代码运行时间超过Tick一秒应该花的时间
                else
                    deltaTime = (float)runTime / 1000;
            }
        }

        private void CreateGameObjects()
        {
            Assembly assembly = RuntimeReflector.GetAssembly;

            List<KeyValuePair<uint, object>> pairs = new List<KeyValuePair<uint, object>>();
            //获取游戏物体上的脚本
            foreach (var type in assembly.GetTypes())
            {
                CreatGameObject creatGameObject = type.GetCustomAttribute<CreatGameObject>();
                //是否继承Script并且创建游戏物体
                if (type.IsSubclassOf(typeof(Script)) && creatGameObject != null)
                {
                    pairs.Add(new KeyValuePair<uint, object>(creatGameObject.CreatOrder, type));
                }
            }
            //Sorting(order越小的越先调用)
            Mathematics.InsertionSort(pairs);
            foreach (KeyValuePair<uint, object> pair in pairs)
            {
                Type type = (Type)pair.Value;
                CreatGameObject creatGameObject = type.GetCustomAttribute<CreatGameObject>();

                //调用构造方法
                GameObject gameObject = new GameObject(creatGameObject.Name);
                //添加脚本组件(脚本必须包含public无参构造方法, 并且这里会调用一次构造)
                gameObject.AddComponent(type);
                //添加required组件
                foreach (Type each in creatGameObject.RequiredComponents)
                {
                    //如果继承Component类型(必须包含public无参构造方法)
                    if (each.IsSubclassOf(typeof(Component)))
                        gameObject.AddComponent(each);
                }
            }
        }

        private void UpdateGameObjects()
        {
            //GameObject在List中的位置将会影响游戏物体在生命周期中的更新顺序
            InvokeSystem.Update();
            CallScriptMethod(gameObjects, "Start", true);   //调用Start
            CallScriptMethod(gameObjects, "Update");        //调用Update
            PhysicsSystem.Update(gameObjects);              //碰撞检测
            NetworkSystem.Update(gameObjects);              //传输消息
            RendererSystem.Update(gameObjects);             //渲染物体
        }

        public static void CallScriptMethod(List<GameObject> gameObjects, string methodName, bool start = false, params object[] parameters)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                
                //反射获取components引用实现动态遍历components
                List<Component> components = (List<Component>)RuntimeReflector.GetPrivateInstanceField(gameObject, "components");

                for (int j = 0; j < components.Count; j++)
                {
                    Component component = components[j];
                    //游戏物体未激活停止执行后续Script
                    if (!gameObject.Active)
                        break;
                    //游戏物体被销毁停止执行后续Script
                    if (!gameObjects.Contains(gameObject))
                        break;
                    //保证组件激活
                    if (!component.Active)
                        continue;
                    //筛选继承Script的组件
                    if (!component.GetType().IsSubclassOf(typeof(Script)))
                        continue;

                    Script script = (Script)component;
                    if (!start && !script.Started) //中途创建, 等待下一个生命周期
                        continue;
                    if (start && script.Started)   //每个脚本的Start只能调用一次
                        continue;
                    if (start)                     //调用Start
                        script.Started = true;
                    RuntimeReflector.InvokePublicInstanceMethod(script, methodName, parameters);
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