using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Destroy
{
    public class Bootstrap
    {
        private int tickPerSecond;
        private bool block;
        private List<object> scriptInstances;

        public Bootstrap(int tickPerSecond, bool block)
        {
            this.tickPerSecond = tickPerSecond;
            this.block = block;
            scriptInstances = new List<object>();

            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (var _class in assembly.GetTypes())
            {
                if (_class.IsSubclassOf(typeof(Script)))
                {
                    object instance = assembly.CreateInstance($"{_class.Namespace}.{_class.Name}");
                    scriptInstances.Add(instance);
                }
            }
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

        private void CallMethod(string methodName, params object[] parameters)
        {
            foreach (var instance in scriptInstances)
            {
                MethodInfo method = instance.GetType().GetMethod(methodName);

                method?.Invoke(instance, parameters);
            }
        }
    }
}