using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Destroy
{
    public class Bootstrap
    {
        private int tickPerSecond;
        private bool block;
        private Assembly assembly;
        private List<object> instances;

        public Bootstrap(int tickPerSecond, bool block)
        {
            this.tickPerSecond = tickPerSecond;
            this.block = block;
            assembly = Assembly.GetExecutingAssembly();
            instances = new List<object>();

            foreach (var _class in assembly.GetTypes())
            {
                if (_class.IsSubclassOf(typeof(Script)))
                {
                    object instance = assembly.CreateInstance($"{_class.Namespace}.{_class.Name}");
                    instances.Add(instance);
                }
            }
        }

        public void Start() => CallMethod("Start");

        public void Tick()
        {
            Thread thread = new Thread
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

            thread.Start();
        }

        private void CallMethod(string methodName, params object[] parameters)
        {
            foreach (var instance in instances)
            {
                MethodInfo method = instance.GetType().GetMethod(methodName);

                method?.Invoke(instance, parameters);
            }
        }
    }
}