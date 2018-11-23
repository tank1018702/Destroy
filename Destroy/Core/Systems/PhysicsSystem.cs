namespace Destroy
{
    using System.Collections.Generic;

    public static class PhysicsSystem
    {
        public static void Update(List<GameObject> gameObjects)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject gameObject = gameObjects[i];
                //获取该碰撞体
                Collider collider = gameObject.GetComponent<Collider>();
                if (!gameObject.Active || collider == null || !collider.Active)
                    continue;

                //遍历其他所有游戏物体进行检测
                for (int j = 0; j < gameObjects.Count; j++)
                {
                    GameObject other = gameObjects[j];
                    //不与自己发生碰撞
                    if (gameObject == other)
                        continue;
                    //获取他人的碰撞体
                    Collider otherCollider = other.GetComponent<Collider>();
                    if (!other.Active || otherCollider == null || !otherCollider.Active)
                        continue;

                    Transform transform = gameObject.GetComponent<Transform>();
                    Transform otherTransform = other.GetComponent<Transform>();
                    //发生碰撞
                    if (transform.Position == otherTransform.Position)
                        RuntimeEngine.CallScriptMethod(gameObject, "OnCollision", false, otherCollider);
                }
            }
        }

        ///// <summary>
        ///// 矩形碰撞检测
        ///// </summary>
        //private static bool RectIntersects(Vector2Int selfPos, Vector2Int otherPos, RectCollider self, RectCollider other)
        //{
        //    if (selfPos.X >= otherPos.X + other.Size.X || otherPos.X >= selfPos.X + self.Size.X)
        //        return false;
        //    else if (selfPos.Y >= otherPos.Y + other.Size.Y || otherPos.Y >= selfPos.Y + self.Size.Y)
        //        return false;
        //    else
        //        return true;
        //}
    }
}