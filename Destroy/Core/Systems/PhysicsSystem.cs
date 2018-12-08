namespace Destroy
{
    using System.Collections.Generic;

    internal static class PhysicsSystem
    {

        public static Dictionary<Vector2Int, Collider> staticColliders = new Dictionary<Vector2Int, Collider>();
        /// <summary>
        /// 初始化,将静态碰撞体加入静态对象中
        /// </summary>
        public static void Init(List<GameObject> gameObjects)
        {
            foreach(var v in gameObjects)
            {
                StaticCollider staticCollider = v.GetComponent<StaticCollider>();
                if (staticCollider != null)
                {
                    foreach(Vector2Int dis in staticCollider.posList)
                    {
                        Vector2Int vi = staticCollider.transform.Position + dis;
                        if (!staticColliders.ContainsKey(vi))
                            staticColliders.Add(vi, staticCollider);
                    }
                }
            }
        }

        private static bool HasInit = false;

        //Update流程 写入静态碰撞体 - 写入动态碰撞体 - 检出rigidbody - 移动rigidbody - 循环检测rigidbody和collider的碰撞
        public static void Update(List<GameObject> gameObjects)
        {
            if(!HasInit)
            {
                PhysicsSystem.Init(gameObjects);
                PhysicsSystem.HasInit = true;
                return;
            }

            Dictionary<Vector2Int, Collider> colliders = new Dictionary<Vector2Int, Collider>();
            List<RigidBody> rigids = new List<RigidBody>();

            foreach (var gameObject in gameObjects)
            {
                //将动态碰撞体加入colliders字典
                MeshCollider mc  = gameObject.GetComponent<MeshCollider>();
                if (mc!=null)
                {
                    foreach(Vector2Int dis in mc.posList)
                    {
                        Vector2Int vi = gameObject.transform.Position + dis;
                        if (!colliders.ContainsKey(vi))
                            colliders.Add(vi, mc);
                    }
                }
                //检出所有rigid准备进行处理
                RigidBody rigid = gameObject.GetComponent<RigidBody>();
                if (rigid != null)
                {
                    rigids.Add(rigid);
                }
            }

            foreach(var rigid in rigids)
            {
                //当前刚体要移动的目标点
                Vector2Int dis = rigid.Move();
                Vector2Int to = rigid.transform.Position + dis;
                //当产生了位移之后,开始检测移动系统
                if (dis != Vector2Int.Zero)
                { 
                    //撞墙了,强制这个rigid停止移动
                    if (staticColliders.ContainsKey(to))
                    {
                        RuntimeEngine.CallScriptMethod(rigid.gameObject, "OnCollision", false, staticColliders[to]);
                        rigid.Stop();
                        return;
                    }
                    //如果撞到了其他活动的colliders
                    else if (colliders.ContainsKey(to))
                    {
                        Collider otherCollider = colliders[to];
                        //如果这个是自己,那么正常移动并返回
                        if (otherCollider == rigid.GetComponent<MeshCollider>())
                        {
                            rigid.transform.Translate(dis);
                            return;
                        }
                        //获得自己的质量
                        float thisMass = rigid.Mass;
                        //获得对方的质量
                        float otherMass;
                        RigidBody otherRigid = otherCollider.GetComponent<RigidBody>();
                        if (otherRigid != null)
                        {
                            otherMass = otherRigid.Mass;

                            //如果自己的质量比对方小,那么自己被阻挡停止
                            if (thisMass <= otherMass)
                            {
                                RuntimeEngine.CallScriptMethod(rigid.gameObject, "OnCollision", false, colliders[to]);
                                rigid.Stop();
                                return;
                            }
                            //如果自己质量比对方大,那么将对方推走,下一帧再自己走
                            else
                            {
                                RuntimeEngine.CallScriptMethod(rigid.gameObject, "OnCollision", false, colliders[to]);
                                otherRigid.transform.Translate(dis);
                                rigid.transform.Translate(dis);
                                //rigid.AddFPosition(new Vector2(dis.X, dis.Y));
                                return;
                            }
                        }
                        //如果没有获取对方的质量,那么强制停止
                        else
                        {
                            RuntimeEngine.CallScriptMethod(rigid.gameObject, "OnCollision", false, colliders[to]);
                            rigid.Stop();
                            return;
                        }

                    }
                    //没有撞墙,那么移动transform
                    else
                    {
                        rigid.transform.Translate(dis);
                    }
                }
            }
            /*
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
            */
        }
    }
}