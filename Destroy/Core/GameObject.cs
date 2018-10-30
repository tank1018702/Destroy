namespace Destroy
{
    using System.Collections.Generic;

    public class GameObject
    {
        private Dictionary<string, Component> components;

        public GameObject()
        {
            components = new Dictionary<string, Component>();
        }

        public T AddComponent<T>() where T : Component, new()
        {
            string name = nameof(T);
            if (components.ContainsKey(name))
                return null;

            T instance = new T();
            components.Add(name, instance);

            return instance;
        }

        public T GetComponent<T>() where T : Component, new()
        {
            string name = nameof(T);
            if (!components.ContainsKey(name))
                return null;

            return components[name] as T;
        }

        public void RemoveComponent<T>() where T : Component, new()
        {
            string name = nameof(T);
            if (!components.ContainsKey(name))
                return;

            components.Remove(name);
        }
    }
}