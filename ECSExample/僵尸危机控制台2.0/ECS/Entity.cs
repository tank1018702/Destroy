using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZombieInfection
{
    /// <summary>
    /// 实体
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 组件标识(包含哪些组件)
        /// </summary>
        public uint ComponentsFlag { get; set; }

        /// <summary>
        /// 组件字典
        /// </summary>
        private Dictionary<string, Component> Components { get; set; }

        public Entity(int id)
        {
            ID = id;
            ComponentsFlag = 0;
            Components = new Dictionary<string, Component>();
        }

        public Entity(int id, params ComponentFlag[] flags)
        {
            ID = id;
            ComponentsFlag = 0;
            Components = new Dictionary<string, Component>();

            AddComponents(flags);
        }

        public Entity(int id, List<ComponentFlag> flags)
        {
            ID = id;
            ComponentsFlag = 0;
            Components = new Dictionary<string, Component>();

            AddComponents(flags.ToArray());
        }

        public bool HasComponent(ComponentFlag flag) => ComponentsFlag.Has((uint)flag);

        public T AddComponent<T>() where T : Component, new()
        {
            string name = typeof(T).Name;
            uint flag = name.GetFlag();

            if (ComponentsFlag.Has(flag))
                throw new Exception("不能添加重复组件");

            T instance = new T { ID = ID, Enable = true };
            Components.Add(name, instance);

            //组件标识更新
            ComponentsFlag = ComponentsFlag.Add(flag);
            Gameplay.Instance.EntityFilter(this);

            return instance;
        }

        public T GetComponent<T>() where T : Component, new()
        {
            string name = typeof(T).Name;
            uint flag = name.GetFlag();

            if (!ComponentsFlag.Has(flag))
                return null;

            return Components[name] as T;
        }

        public void RemoveComponent<T>() where T : Component
        {
            string name = typeof(T).Name;
            uint flag = name.GetFlag();

            if (!ComponentsFlag.Has(flag))
                return;

            Components.Remove(name);

            //组件标识更新
            ComponentsFlag = ComponentsFlag.Remove(flag);
            Gameplay.Instance.EntityFilter(this);
        }

        private void AddComponents(params ComponentFlag[] flags)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var flag in flags)
            {
                string className = flag.ToString(); //获取枚举的字符串
                string namespaceName = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                //创建实例
                Component component = (Component)assembly.CreateInstance($"{namespaceName}.{className}");
                if (component != null)
                    Components.Add(component.GetType().Name, component);
            }
        }
    }
}