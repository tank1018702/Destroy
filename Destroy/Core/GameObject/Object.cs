namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Diagnostics;

    public class Object
    {
        private bool active;

        /// <summary>
        /// 名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active
        {
            get => active;
            set
            {
                if (Name == "Transform" && value == false) //不能禁用Transform
                    return;
                active = value;
            }
        }

        private static List<GameObject> gameObjects = new List<GameObject>();

        /// <summary>
        /// 销毁一个物体
        /// </summary>
        public static void Destroy(Object obj)
        {
            Type type = obj.GetType();
            //不能销毁Transform组件
            if (type == typeof(Transform))
                return;
            if (type.IsSubclassOf(typeof(Component)))
            {
                Component component = (Component)obj;
                List<Component> components = (List<Component>)RuntimeReflector.GetPrivateInstanceField
                    (component.gameObject, "components");

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
                components.Remove(component);
            }
            else
            {
                GameObject gameObject = (GameObject)obj;
                gameObjects.Remove(gameObject);
            }
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