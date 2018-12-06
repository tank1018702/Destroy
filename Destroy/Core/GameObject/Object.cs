namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Diagnostics;

    public class Object
    {
        private string name;
        private bool active;

        /// <summary>
        /// 名字
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// 激活
        /// </summary>
        public bool Active
        {
            get => active;
            set
            {
                //不能禁用继承IPersistent接口的对象
                if (typeof(IPersistent).IsAssignableFrom(GetType()) && value == false) 
                    return;
                active = value;
            }
        }

        internal static List<GameObject> GameObjects = new List<GameObject>();

        /// <summary>
        /// 销毁一个物体
        /// </summary>
        public static void Destroy(Object obj)
        {
            Type type = obj.GetType();
            //不能销毁继承IPersistent接口的对象
            if (typeof(IPersistent).IsAssignableFrom(type))
                return;

            //销毁组件
            if (type.IsSubclassOf(typeof(Component)))
            {
                Component component = (Component)obj;
                List<Component> components = component.gameObject.Components;

                //获取调用该方法的方法
                StackTrace stackTrace = new StackTrace(true);
                MethodBase method = stackTrace.GetFrame(1).GetMethod();
                string name = method.DeclaringType.Name;
                //不能移除调用方法的脚本, 只禁用
                if (name == component.Name)
                {
                    component.Active = false;
                    return;
                }

                //移除该组件
                components.Remove(component);
            }
            //销毁游戏物体
            else
            {
                GameObject gameObject = (GameObject)obj;
                GameObjects.Remove(gameObject);
            }
        }

        /// <summary>
        /// 延迟销毁一个物体
        /// </summary>
        public static void Destroy(Object obj, float delayTime)
        {
            InvokeSystem.AddDelayAction(() => Destroy(obj), delayTime);
        }

        public static implicit operator bool(Object exists)
        {
            if (exists != null)
                return true;
            else
                return false;
        }
    }
}