namespace Destroy
{
    using System;
    using System.Collections.Generic;

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
        /// 在合适的时机禁用并销毁一个物体
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