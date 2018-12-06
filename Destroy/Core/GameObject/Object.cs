namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Diagnostics;

    //Don't modify this
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

        internal static bool DestroyedComponent;

        internal static bool DestroyedGameObject;

        /// <summary>
        /// 销毁一个物体
        /// </summary>
        public static void Destroy(Object obj)
        {
            Type type = obj.GetType();
            //不能销毁继承IPersistent接口的对象
            if (typeof(IPersistent).IsAssignableFrom(type))
                return;

            //获取调用该方法的类
            StackTrace stackTrace = new StackTrace(true);
            Type scriptType = stackTrace.GetFrame(1).GetMethod().DeclaringType;

            //销毁组件
            if (type.IsSubclassOf(typeof(Component)))
            {
                Component component = (Component)obj;
                GameObject gameObject = component.gameObject;
                //自己销毁自己脚本
                bool SelfComponent = gameObject.GetComponent(scriptType) != null;
                if (SelfComponent)
                    DestroyedComponent = true;

                //移除该组件
                List<Component> components = gameObject.Components;
                components.Remove(component);
            }
            //销毁游戏物体
            else
            {
                GameObject gameObject = (GameObject)obj;
                //自己销毁自己游戏物体
                bool selfGameObject = gameObject.GetComponent(scriptType) != null;
                if (selfGameObject)
                    DestroyedGameObject = true;

                //移除该游戏物体
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