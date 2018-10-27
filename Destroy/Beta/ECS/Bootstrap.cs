using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Destroy.ECS
{
    public sealed class Bootstrap
    {
        static Assembly assembly;
        static List<Type> classes;

        public static void Run()
        {
            assembly = Assembly.GetExecutingAssembly();

            classes = new List<Type>();

            foreach (var _class in assembly.GetTypes())
            {
                if (_class.IsSubclassOf(typeof(System)))
                    classes.Add(_class);
            }
        }

        public static void InvokeSystem()
        {
            foreach (var _class in classes)
            {
                object instance = assembly.CreateInstance($"{_class.Namespace}.{_class.Name}");

                MethodInfo method = instance.GetType().GetMethod("OnUpdate");

                method?.Invoke(instance, null);
            }
        }
    }
}