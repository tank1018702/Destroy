namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    public class RuntimeEngine
    {
        private static Assembly assembly;

        internal static Assembly GetAssembly
        {
            get
            {
                if (assembly == null)
                    assembly = Assembly.GetEntryAssembly();
                return assembly;
            }
        }

        public Thread GameThread { get; private set; }

        public event Action OnInitialized;

        private readonly List<GameObject> gameObjects;

        private Setting.Config config;

        public RuntimeEngine()
        {
            GameThread = null;
            gameObjects = new List<GameObject>();
            Manage += gameObject => gameObjects.Add(gameObject);
            Object.GameObjects = gameObjects;
            //进行配置初始化
            OnInitialized += () =>
            {
                config = Setting.Load();
                //进行配置
                
            };
        }

        public void Run(int tickPerSecond, bool allowMultiple = true)
        {
            if (!allowMultiple) //Singleton Check
            {
                Mutex mutex = new Mutex(true, Process.GetCurrentProcess().ProcessName, out bool nonexsitent);
                if (!nonexsitent)
                    Environment.Exit(0);
            }

            GameThread = Thread.CurrentThread;
            GameThread.Name = "GameThread";

            OnInitialized?.Invoke();    //显式初始化
            CreateGameObjects();        //使用CreatGameObject初始化

            Stopwatch stopwatch = new Stopwatch();
            int tickTime = 1000 / tickPerSecond < 1 ? 1 : 1000 / tickPerSecond; //每帧因该花的时间(最少应为1毫秒)
            float deltaTime = 0; //这一帧距离上一帧的时间
            float totalTime = 0; //总执行时间

            while (true)
            {
                //开始计时
                stopwatch.Restart();
                //设置Time类属性
                totalTime += deltaTime;
                Time.DeltaTime = deltaTime;
                Time.TotalTime = totalTime;

                UpdateGameObjects();

                //计算应该休眠的时间, 保证每秒运行相应Tick次数
                int runTime = (int)stopwatch.ElapsedMilliseconds;
                int delayTime = tickTime - runTime;
                if (delayTime > 0)
                {
                    deltaTime = (float)tickTime / 1000;
                    Thread.Sleep(delayTime); //不是高精度计时器, 只能起到模拟的效果
                }
                else
                    deltaTime = (float)runTime / 1000;
            }
        }

        private void CreateGameObjects()
        {
            Assembly assembly = GetAssembly;

            List<KeyValuePair<uint, object>> pairs = new List<KeyValuePair<uint, object>>();

            foreach (var type in assembly.GetTypes())
            {
                CreatGameObject creatGameObject = type.GetCustomAttribute<CreatGameObject>();
                //继承Script并且使用特性[CreatGameObject]
                if (type.IsSubclassOf(typeof(Script)) && creatGameObject != null)
                    pairs.Add(new KeyValuePair<uint, object>(creatGameObject.CreatOrder, type));
            }

            Mathematics.QuickSort(pairs);

            foreach (KeyValuePair<uint, object> pair in pairs)
            {
                Type type = (Type)pair.Value;
                CreatGameObject creatGameObject = type.GetCustomAttribute<CreatGameObject>();

                GameObject gameObject = new GameObject(creatGameObject.Name); //调用构造方法
                gameObject.AddComponent(type); //添加脚本组件(脚本必须包含public无参构造方法, 并且这里会调用一次构造)
            }
        }

        private void UpdateGameObjects()
        {
            //GameObject在List中的位置将会影响游戏物体在生命周期中的更新顺序
            InvokeSystem.Update();
            CallScriptMethod(gameObjects, "Start", true);   //调用Start
            CallScriptMethod(gameObjects, "Update");        //调用Update
            PhysicsSystem.Update(gameObjects);              //碰撞检测

            //这个就很尴尬了.. 它必须在物理系统之后,在渲染系统之前
            Camera.main.LateUpdate();

            NetworkSystem.Update(gameObjects);              //传输消息
            RendererSystem.Update(gameObjects);             //渲染物体


        }

        internal static Action<GameObject> Manage;

        internal static void CallScriptMethod(List<GameObject> gameObjects, string methodName, bool start = false, params object[] parameters)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                List<Component> components = gameObject.Components;

                for (int j = 0; j < components.Count; j++)
                {
                    Component component = components[j];
                    //游戏物体未激活停止执行后续脚本
                    if (!gameObject.Active)
                        break;
                    //游戏物体被销毁停止执行后续脚本
                    if (!gameObjects.Contains(gameObject))
                        break;
                    //保证脚本未激活继续执行后续脚本
                    if (!component.Active)
                        continue;
                    //筛选继承Script的脚本
                    if (!component.GetType().IsSubclassOf(typeof(Script)))
                        continue;

                    Script script = (Script)component;
                    if (!start && !script.Started) //Start之后创建, 等待下一个生命周期Start
                        continue;
                    if (start && script.Started)   //每个脚本的Start只能调用一次
                        continue;
                    if (start)                     //调用Start
                        script.Started = true;
                    //调用该方法
                    script.GetType().GetMethod(methodName).
                        Invoke(script, BindingFlags.Public | BindingFlags.Instance, null, parameters, null);
                    //保证执行顺序不会出现问题
                    if (Object.DestroyedComponent)
                    {
                        j--;
                        Object.DestroyedComponent = false;
                    }
                    if (Object.DestroyedGameObject)
                    {
                        i--;
                        Object.DestroyedGameObject = false;
                    }
                }
            }
        }

        internal static void CallScriptMethod(GameObject gameObject, string methodName, bool start = false, params object[] parameters)
        {
            List<GameObject> gameObjects = new List<GameObject> { gameObject };
            CallScriptMethod(gameObjects, methodName, start, parameters);
        }
    }
}