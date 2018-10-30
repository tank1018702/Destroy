using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Destroy
{
    public class RuntimeEngine
    {
        private int tickPerSecond;
        private bool block;
        private List<object> scriptInstances;


        public RuntimeEngine(int tickPerSecond, bool block)
        {
            this.tickPerSecond = tickPerSecond;
            this.block = block;
            scriptInstances = new List<object>();

            Assembly assembly = Assembly.GetExecutingAssembly();

            List<OrderClass> orderClasses = new List<OrderClass>();

            foreach (var _class in assembly.GetTypes())
            {
                if (_class.IsSubclassOf(typeof(Script)))
                {
                    OrderClass orderClass = new OrderClass(uint.MaxValue, _class);
                    UpdateOrder updateOrder = _class.GetCustomAttribute<UpdateOrder>();
                    //指定了顺序
                    if (updateOrder != null)
                        orderClass.Order = updateOrder.Order;

                    orderClasses.Add(orderClass);
                }
            }
            //Sorting(order越小的越先执行)
            foreach (var orderClass in InsertionSort(orderClasses))
            {
                //创建对象
                object instance = assembly.CreateInstance($"{orderClass.Type.Namespace}.{orderClass.Type.Name}");
                //设置GameObject
                instance.GetType().GetField("GameObject").SetValue(instance, new GameObject());
                //添加进队列
                scriptInstances.Add(instance);
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
                //调用方法, 传递参数
                method?.Invoke(instance, parameters);
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