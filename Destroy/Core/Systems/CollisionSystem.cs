namespace Destroy
{
    using System.Collections.Generic;

    public static class CollisionSystem
    {
        public static void Update(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                //获取该碰撞体
                Collider collider = gameObject.GetComponent<Collider>();
                if (collider == null || !collider.Initialized)
                    continue;

                //遍历其他所有游戏物体进行检测
                foreach (GameObject other in gameObjects)
                {
                    //不与自己发生碰撞
                    if (gameObject == other)
                        continue;
                    Collider otherCollider = other.GetComponent<Collider>();
                    if (otherCollider == null || !otherCollider.Initialized)
                        continue;

                    Transform transform = gameObject.GetComponent<Transform>();
                    Transform otherTransform = other.GetComponent<Transform>();

                    //判断自己是哪种碰撞体
                    string name = collider.GetType().Name;
                    //判断敌人是哪种碰撞体
                    string otherName = otherCollider.GetType().Name;

                    //双方都是矩形碰撞体
                    if (name == nameof(RectCollider) && otherName == name)
                    {
                        RectCollider rectCollider = collider as RectCollider;
                        RectCollider otherRectCollider = otherCollider as RectCollider;
                        //碰撞检测
                        if (RectIntersects(transform.Position, otherTransform.Position, rectCollider, otherRectCollider))
                        {
                            //调用该碰撞体的OnCollision方法
                            RuntimeEngine.CallScriptMethod(gameObject, "OnCollision", false, otherCollider);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 矩形碰撞检测
        /// </summary>
        private static bool RectIntersects(Vector2Int selfPos, Vector2Int otherPos, RectCollider self, RectCollider other)
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