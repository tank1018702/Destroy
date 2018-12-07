namespace Destroy.Standard
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 使用该方法创建一个游戏物体
    /// </summary>
    public delegate GameObject Instantiate();

    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPool
    {
        private Instantiate instantiate;

        private readonly List<GameObject> pool;

        public ObjectPool(Instantiate instantiate)
        {
            this.instantiate = instantiate;
            pool = new List<GameObject>();
        }

        public void PreAllocate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject instance = instantiate();
                ReturnInstance(instance);
            }
        }

        public GameObject GetInstance()
        {
            if (pool.Any())
            {
                GameObject instance = pool.First();
                pool.Remove(instance);
                instance.Active = true;
                return instance;
            }
            return instantiate();
        }

        public void ReturnInstance(GameObject instance)
        {
            instance.Active = false;
            pool.Add(instance);
        }
    }
}