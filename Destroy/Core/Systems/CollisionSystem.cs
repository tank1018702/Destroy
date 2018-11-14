namespace Destroy
{
    using System.Collections.Generic;

    public static class CollisionSystem
    {
        public static void Update(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                Transform transform = gameObject.GetComponent<Transform>();
                Collider collider = gameObject.GetComponent<Collider>();
                if (transform == null || collider == null)
                    continue;
                //遍历其他所有游戏物体进行检测
                foreach (GameObject other in gameObjects)
                {
                    //不与自己发生碰撞
                    if (gameObject == other)
                        continue;
                    Transform otherTransform = other.GetComponent<Transform>();
                    Collider otherCollider = other.GetComponent<Collider>();
                    if (otherTransform == null || otherCollider == null)
                        continue;
                    //碰撞检测
                    if (Intersects(transform.Position, otherTransform.Position, collider, otherCollider))
                    {
                        //调用该碰撞体的OnCollision方法
                        RuntimeEngine.CallScriptMethod(gameObject, "OnCollision", false, otherCollider);
                    }
                }
            }
        }

        private static bool Intersects(Vector2Int selfPos, Vector2Int otherPos, Collider self, Collider other)
        {
            if (selfPos.X >= otherPos.X + other.Size.X || otherPos.X >= selfPos.X + self.Size.X)
                return false;
            else if (selfPos.Y >= otherPos.Y + other.Size.Y || otherPos.Y >= selfPos.Y + self.Size.Y)
                return false;
            else
                return true;
        }
    }
}