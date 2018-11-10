namespace Destroy
{
    using System.Collections.Generic;

    public static class ScriptSystem
    {
        public static int GameObjectCount => gameObjects.Count;

        private static List<GameObject> gameObjects;

        public static void InvokeScript(List<GameObject> gameObjects)
        {
            ScriptSystem.gameObjects = gameObjects;

            //统一调用Start
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                //反射获取components引用实现动态遍历components
                List<Component> components = (List<Component>)RuntimeReflector.GetPrivateField(gameObject, "components");

                for (int j = 0; j < components.Count; j++)
                {
                    //如果游戏物体被销毁则停止执行后续Start
                    if (!gameObjects.Contains(gameObject))
                        break;

                    Component component = components[j];
                    //筛选继承Script的组件
                    if (!component.GetType().IsSubclassOf(typeof(Script)))
                        continue;
                    Script script = (Script)component;

                    if (!script.Started)
                    {
                        //在Start中创建的Script会在随后调用其Start
                        script.Started = true;
                        script.Start(); //如果在Start中改了Started就意味着可以调用多次Start方法。
                    }
                }
            }

            //统一调用Update
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                //反射获取components引用实现动态遍历components
                List<Component> components = (List<Component>)RuntimeReflector.GetPrivateField(gameObject, "components");

                for (int j = 0; j < components.Count; j++)
                {
                    //如果游戏物体被销毁则停止执行后续Update
                    if (!gameObjects.Contains(gameObject))
                        break;

                    Component component = components[j];
                    //筛选继承Script的组件
                    if (!component.GetType().IsSubclassOf(typeof(Script)))
                        continue;
                    Script script = (Script)component;

                    //在Update中创建的Script会在下一次调用Start时调用其Start方法
                    script.Update();
                }
            }
        }
    }
}