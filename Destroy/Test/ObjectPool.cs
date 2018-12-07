namespace Destroy.Test
{
    using System.Collections.Generic;
    using System.Linq;

    public class ObjectPool
    {
        private GameObject prefab;

        private readonly List<GameObject> pool;

        public ObjectPool(GameObject prefab)
        {
            this.prefab = prefab;
            pool = new List<GameObject>();
        }

        public void PreAllocate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject instance = new GameObject(); //TODO Copy Prefab
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
            return new GameObject(); //TODO Copy Prefab
        }

        public void ReturnInstance(GameObject instance)
        {
            instance.Active = false;
            pool.Add(instance);
        }
    }
}