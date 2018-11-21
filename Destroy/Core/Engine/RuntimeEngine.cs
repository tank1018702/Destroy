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
            NewGameObject += gameObject => gameObjects.Add(gameObject);
            //注入gameObjects的引用
            GameObject temp = new GameObject();
            RuntimeReflector.SetPrivateStaticField(temp, "gameObjects", gameObjects);
            GameObject.Destroy(temp);
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
            
            CreateGameObjects();

            Stopwatch stopwatch = new Stopwatch();
            Time time = new Time();
            //每帧因该花的时间(最少应为1毫秒)
            int tickTime = 1000 / tickPerSecond < 1 ? 1 : 1000 / tickPerSecond;
            //脚本运行花费时间
            int runTime = 0;
            //线程休眠时间
            int delayTime = 0;
            //这一帧距离上一帧的时间
            float deltaTime = 0;

            while (true)
            {
                //开始计时
                stopwatch.Restart();
                //设置Time类属性
                RuntimeReflector.SetPublicStaticProperty(time, "DeltaTime", deltaTime);

                UpdateGameObjects();

                //计算应该休眠的时间, 保证每秒运行相应Tick次数
                runTime = (int)stopwatch.ElapsedMilliseconds;
                delayTime = tickTime - runTime;
                if (delayTime > 0)
                {
                    deltaTime = (float)(runTime + delayTime) / 1000;
                    Thread.Sleep(delayTime);
                }
                //代码运行时间超过Tick一秒应该花的时间
                else
                    deltaTime = (float)runTime / 1000;
            }
        }

        private void CreateGameObjects()
        {
            Assembly assembly = Assembly.GetEntryAssembly(); //获取调用该方法的程序集而不是引擎所在的程序集

            List<KeyValuePair<uint, object>> pairs = new List<KeyValuePair<uint, object>>();
            //获取游戏物体上的脚本
            foreach (var _class in assembly.GetTypes())
            {
                CreatGameObject creatGameObject = _class.GetCustomAttribute<CreatGameObject>();
                //是否继承Script并且创建游戏物体
                if (_class.IsSubclassOf(typeof(Script)) && creatGameObject != null)
                {
                    pairs.Add(new KeyValuePair<uint, object>(creatGameObject.CreatOrder, _class));
                }
            }
            //Sorting(order越小的越先调用)
            Mathematics.InsertionSort(pairs);
            foreach (KeyValuePair<uint, object> item in pairs)
            {
                Type type = (Type)item.Value;
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
            //LifeCycle
            CallScriptMethod(gameObjects, "Start", true);   //调用Start
            CallScriptMethod(gameObjects, "Update");        //调用Update
            //PhysicsSystem.Update(gameObjects);              //碰撞检测
            //RendererSystem.Update(gameObjects);             //渲染物体
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
                    //筛选继承Script的组件
                    if (!component.GetType().IsSubclassOf(typeof(Script)))
                        continue;
                    //如果游戏物体被销毁则停止执行后续Script
                    if (!gameObjects.Contains(gameObject))
                        break;
                    Script script = (Script)component;
                    //每个脚本的Start只能调用一次
                    if (start && script.Started)
                        continue;
                    if (start)
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