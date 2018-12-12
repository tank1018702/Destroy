namespace Destroy
{
    using System.Collections.Generic;

    public delegate void Behavior(object obj);

    public class BehaviorQueue
    {
        private bool stop;

        private List<Behavior> behaviors;

        public BehaviorQueue() => behaviors = new List<Behavior>();

        public BehaviorQueue Add(Behavior behavior)
        {
            if (behavior != null)
                behaviors.Add(behavior);
            return this;
        }

        public BehaviorQueue Remove(Behavior behavior)
        {
            behaviors.Remove(behavior);
            return this;
        }

        public void Do(object obj)
        {
            stop = false;
            foreach (var behavior in behaviors)
            {
                if (stop)
                    break;
                behavior(obj);
            }
        }

        public void Stop() => stop = true;
    }
}