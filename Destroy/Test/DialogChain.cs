using System;
using System.Collections;
using System.Collections.Generic;

namespace Destroy
{
    public delegate void Behavior(object obj);

    public class BehaviorQueue
    {
        //用委托字段简化方法声明 TestOnly
        private readonly Action Clear = Console.Clear; 

        private bool stop;

        private List<Behavior> behaviors;

        public BehaviorQueue()
        {
            behaviors = new List<Behavior>();
        }

        public void Add(Behavior behavior)
        {
            //不能存在空的行为队列
            if (behavior != null)
                behaviors.Add(behavior);
        }

        public void Stop() => stop = true;

        public void Do(object obj)
        {
            stop = false;
            foreach (var each in behaviors)
            {
                if (stop)
                    break;
                try
                {
                    each(obj);
                }
                catch (Exception ex)
                {
                    Debug.Error(ex.Message);
                    Stop();
                }
            }
        }
    }
}