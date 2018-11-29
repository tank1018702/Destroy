namespace Destroy
{
    using System.Collections.Generic;

    internal static class PhysicsSystem
    {
        public static void Update(List<GameObject> gameObjects)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                if (!gameObject.Active)
                    continue;
                Collider collider = gameObject.GetComponent<Collider>();
                if (!collider || !collider.Active)
                    continue;

                //遍历其他所有游戏物体进行检测
                for (int j = 0; j < gameObjects.Count; j++)
                {
                    GameObject other = gameObjects[j];
                    //不与自己发生碰撞
                    if (gameObject == other)
                        continue;
                    if (!other.Active)
                        continue;
                    Collider otherCollider = other.GetComponent<Collider>();
                    if (!otherCollider || !otherCollider.Active)
                        continue;

                    Transform transform = gameObject.GetComponent<Transform>();
                    Transform otherTransform = other.GetComponent<Transform>();
                    //发生碰撞
                    if (transform.Position == otherTransform.Position)
                        RuntimeEngine.CallScriptMethod(gameObject, "OnCollision", false, otherCollider);
                }
            }
        }
    }
}