using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Destroy
{
    public class Bootstrap
    {
        private Assembly assembly;
        private List<Type> classes;
        private Thread thread;

        public Bootstrap()
        {
            assembly = Assembly.GetExecutingAssembly();

            classes = new List<Type>();

            foreach (var _class in assembly.GetTypes())
            {
                if (_class.IsSubclassOf(typeof(Scriptable)))
                    classes.Add(_class);
            }
        }

        public void Start() => CallMethods("Start");

        public void Tick()
        {
            thread = new Thread(_Tick) { IsBackground = true };
            thread.Start();
        }

        private void _Tick()
        {
            while (true)
            {
                CallMethods("Update");
                Thread.Sleep(16);
            }
        }

        private void CallMethods(string methodName)
        {
            foreach (var _class in classes)
            {
                object instance = assembly.CreateInstance($"{_class.Namespace}.{_class.Name}");

                MethodInfo method = instance.GetType().GetMethod(methodName);

                method?.Invoke(instance, null);
            }
        }
    }
}