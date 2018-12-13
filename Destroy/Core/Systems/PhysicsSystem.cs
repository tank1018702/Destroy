/*
 * TODO: 加入Mesh旋转功能.Rotate 然后就可以做一个俄罗斯方块了.使用全物理模拟
 * 
 * 12/13
 * 优化了多点碰撞和循环碰撞问题. 
 * 更新了一套超吊的质量运算系统.
 * 通过递归检测,现在可以随便推了,怎么推都行
 * 
 */

namespace Destroy
{
    using System.Collections.Generic;

    internal static class PhysicsSystem
    {
        //都是只读的,外部不能更改,只能由系统自己进行更改 
        public static Dictionary<Vector2Int, Collider> staticColliders { get; private set; } 
        public static Dictionary<Vector2Int, Collider> colliders { get; private set; }
        /// <summary>
        /// 初始化,将静态碰撞体加入静态对象中
        /// </summary>
        public static void Init(List<GameObject> gameObjects)
        {
            staticColliders = new Dictionary<Vector2Int, Collider>();
            colliders = new Dictionary<Vector2Int, Collider>();
            foreach (var v in gameObjects)
            {
                StaticCollider staticCollider = v.GetComponent<StaticCollider>();
                if (staticCollider != null)
                {
                    foreach (Vector2Int dis in staticCollider.ColliderList)
                    {
                        Vector2Int vi = staticCollider.transform.Position + dis;
                        if (!staticColliders.ContainsKey(vi))
                            staticColliders.Add(vi, staticCollider);
                        Debug.Log(vi);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rigid">当前rigid</param>
        /// <param name="to">要前往的目标点</param>
        /// <param name="dis">要进行的位移</param>
        /// <returns></returns>
        public static bool CanMoveIn(RigidBody rigid,Vector2Int to,Vector2Int dis,float mass)
        {
            //撞到静态碰撞体,强制这个rigid停止移动
            if (staticColliders.ContainsKey(to))
            {
                rigid.Stop();
                return false;
            }
            //如果撞到了其他的colliders 且这个碰撞体不是自己
            else if (colliders.ContainsKey(to))
            {
                if(colliders[to] == rigid.GetComponent<Collider>())
                {
                    return true;
                }
                //发生碰撞的另一个碰撞体
                Collider otherCollider = colliders[to];
                //获得自己的质量
                float thisMass = mass;
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
                        return false;
                    }
                    //如果自己质量比对方大,那么将对方推走,并把自己推走
                    else
                    {
                        //递归调用 调用的时候同样会自动检测并移动
                        if(CanMove(otherRigid,dis,mass - otherMass))
                        {
                            //RuntimeEngine.CallScriptMethod(rigid.gameObject, "OnCollision", false, colliders[to]);
                            return true;
                        }
                        else
                        {
                            RuntimeEngine.CallScriptMethod(rigid.gameObject, "OnCollision", false, colliders[to]);
                            return false;
                        }
                    }
                }
                //如果没有获取对方的质量,那么强制停止
                else
                {
                    RuntimeEngine.CallScriptMethod(rigid.gameObject, "OnCollision", false, colliders[to]);
                    rigid.Stop();
                    return false; ;
                }

            }
            //没有撞墙,那么移动transform
            else
            {
                return true;
            }
        }

        //这个Rigid是否可以向目标点移动
        public static bool CanMove(RigidBody rigid, Vector2Int dis,float mass)
        {
            Mesh mesh = rigid.GetComponent<Mesh>();

            //如果这是个单点Mesh
            if (mesh.isSingleMesh)
            {
                //目标点坐标
                Vector2Int to = rigid.transform.Position + dis;
                //如果目的地可以移动,则移动坐标
                if (CanMoveIn(rigid,to,dis,mass))
                {
                    rigid.transform.Translate(dis);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            //如果不是单点Mesh
            else
            {
                foreach(var v in rigid.GetComponent<Mesh>().posList)
                {
                    //目标点坐标
                    Vector2Int to = rigid.transform.Position + dis + v;
                    //如果目的地不可移动,返回
                    if (!CanMoveIn(rigid, to,dis,mass))
                    {
                        return false;
                    }
                    else
                    {

                    }
                }

                foreach (var v in rigid.GetComponent<Mesh>().posList)
                {
                    //目标点坐标
                    Vector2Int to = rigid.transform.Position + dis + v;
                    //动态更改碰撞体缓存
                    //colliders.Remove(rigid.transform.Position);
                    if (colliders.ContainsKey(to))
                    {
                        colliders[to] = rigid.GetComponent<Collider>();
                    }
                    else
                    {
                        colliders.Add(to, rigid.GetComponent<Collider>());
                    }
                }

                //所有的点都通过了检测,那么可以移动
                rigid.transform.Translate(dis);
                return true;
            }
            
        }

        public static bool HasInit = false;

        //Update流程 写入静态碰撞体 - 写入动态碰撞体 - 检出rigidbody - 移动rigidbody - 循环检测rigidbody和collider的碰撞
        public static void Update(List<GameObject> gameObjects)
        {
            if(!HasInit)
            {
                Init(gameObjects);
                HasInit = true;
            }

            colliders = new Dictionary<Vector2Int, Collider>();
            List<RigidBody> rigids = new List<RigidBody>();

            foreach (var gameObject in gameObjects)
            {
                //将动态碰撞体加入colliders字典
                MeshCollider mc = gameObject.GetComponent<MeshCollider>();
                if (mc != null && mc.Active)
                {
                    foreach (Vector2Int dis in mc.ColliderList)
                    {
                        Vector2Int vi = gameObject.transform.Position + dis;
                        if (!colliders.ContainsKey(vi))
                            colliders.Add(vi, mc);
                    }
                }
                //检出所有rigid准备进行处理
                RigidBody rigid = gameObject.GetComponent<RigidBody>();
                if (rigid != null && rigid.Active)
                {
                    rigids.Add(rigid);
                }
            }

            foreach (var rigid in rigids)
            {
                //当前刚体要移动的目标点
                Vector2Int dis = rigid.Move();
                Vector2Int to = rigid.transform.Position + dis;
                //当产生了位移之后,开始检测碰撞系统
                if (dis != Vector2Int.Zero)
                {
                    CanMove(rigid, dis,rigid.Mass);
                }
            }
        }
    }
}