namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Diagnostics;

    public class Object
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active;

        private static List<GameObject> gameObjects = new List<GameObject>();

        /// <summary>
        /// 禁用并销毁一个物体
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
                component.Active = false;
                List<Component> components = (List<Component>)RuntimeReflector.GetPrivateInstanceField
                    (component.gameObject, "components");

                //获取调用该方法的方法
                StackTrace stackTrace = new StackTrace(true);
                MethodBase method = stackTrace.GetFrame(1).GetMethod();
                string name = method.DeclaringType.Name;
                //不能移除调用方法的脚本
                if (name == component.Name)
                    return;

                components.Remove(component);
            }
            else
            {
                GameObject gameObject = (GameObject)obj;
                gameObject.Active = false;
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