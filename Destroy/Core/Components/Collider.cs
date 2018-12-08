using System.Collections.Generic;

namespace Destroy
{
    /*
    * 12/8 by Kyasever
    * RigidBody 控制移动和碰撞
    * Collider应该分为两个组件
    * MeshCollider 自动检测Renderer组件并自动生成Collider
    * Static 初始化的时候就存在. 只要标记为static,就永远都在
    * 之后版本要重新优化
    */
    public abstract class Collider : Component
    {
        public Collider() { }
    }

    /// <summary>
    /// 静态碰撞体,包含这个组件的对象只能在游戏开始的时候创建
    /// 会进行缓冲处理,提升系统性能.
    /// </summary>
    public class StaticCollider: Collider
    {
        public List<Vector2Int> posList;
        public StaticCollider()
        {
            posList = new List<Vector2Int>();
            posList.Add(new Vector2Int(0, 0));
        }
        /// <summary>
        /// 直接初始化Collider
        /// </summary>
        public void Init(List<Vector2Int> l)
        {
            posList = l;
        }
        /// <summary>
        /// 通过Renderer初始化Collider
        /// </summary>
        public void InitWithRenderer(Renderer r)
        {
            //添加当前点
            if (r.GetType() == typeof(PosRenderer))
            {
                posList.Add(new Vector2Int(0, 0));
            }
            //添加一排碰撞体
            else if (r.GetType() == typeof(StringRenderer))
            {
                int n = 0;
                for (int i = 0; i < ((StringRenderer)r).length; i += Camera.main.CharWidth)
                {
                    posList.Add(new Vector2Int(n, 0));
                    n++;
                }
            }
            //添加组碰撞体
            else if (r.GetType() == typeof(GroupRenderer))
            {
                GroupRenderer gr = r as GroupRenderer;
                foreach (var v in gr.list)
                {
                    if (v.Key.GetType() == typeof(PosRenderer))
                    {
                        posList.Add(v.Value);
                    }
                    else if(v.Key.GetType() == typeof(StringRenderer))
                    {
                        int n = 0;
                        for (int i = 0; i < ((StringRenderer)v.Key).length; i += Camera.main.CharWidth)
                        {
                            posList.Add(new Vector2Int(n, 0) + v.Value);
                            n++;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 拟合碰撞体,自动根据身上的Mesh组件生成对应的Collier
    /// </summary>
    public class MeshCollider : Collider
    {
        /// <summary>
        /// 保存着碰撞体信息,初始化之后可以选择再进行更改,暂时不支持其他种类的Collider;
        /// </summary>
        public List<Vector2Int> posList;
        public bool Init()
        {
            Renderer r = GetComponent<Renderer>();
            if (r == null)
            {
                Debug.Warning(gameObject.Name + "对象没有Renderer组件");
                return false;
            }
            posList = new List<Vector2Int>();
            //添加当前点
            if (r.GetType() == typeof(PosRenderer))
            {
                posList.Add(new Vector2Int(0, 0));
            }
            //添加一排碰撞体
            else if (r.GetType() == typeof(StringRenderer))
            {
                int n = 0;
                for (int i = 0; i < ((StringRenderer)r).length; i += Camera.main.CharWidth)
                {
                    posList.Add(new Vector2Int(n, 0));
                    n++;
                }
            }
            //添加组碰撞体
            else if (r.GetType() == typeof(GroupRenderer))
            {
                GroupRenderer gr = r as GroupRenderer;
                foreach (var v in gr.list)
                {
                    if (v.Key.GetType() == typeof(PosRenderer))
                    {
                        posList.Add(v.Value);
                    }
                    else if (v.Key.GetType() == typeof(StringRenderer))
                    {
                        int n = 0;
                        for (int i = 0; i < ((StringRenderer)v.Key).length; i += Camera.main.CharWidth)
                        {
                            posList.Add(new Vector2Int(n, 0) + v.Value);
                            n++;
                        }
                    }
                }
            }
            return true;
        }
        public MeshCollider()
        {
            
        }

    }

    public class RigidBody : Component
    {
        //刚体的质量,可以决定碰撞.
        public float Mass = 1;
        public Vector2 FPosition;
        public Vector2 MoveSpeed;
        public RigidBody()
        {
            Mass = 1;
            FPosition = Vector2.Zero;
            MoveSpeed = Vector2.Zero;
        }

        /// <summary>
        /// 设置速度
        /// </summary>
        public void SetSpeed(Vector2 vector)
        {
            MoveSpeed = vector;
        }
        /// <summary>
        /// 添加一个持续的力的作用.待更新其他模式
        /// </summary>
        internal void AddForce(Vector2 vector)
        {
            MoveSpeed += vector;
        }


        /// <summary>
        /// 停止移动并清除偏移浮点缓存
        /// </summary>
        public void Stop()
        {
            FPosition = Vector2.Zero;
            MoveSpeed = Vector2.Zero;
        }

        /// <summary>
        /// 向movespeed方向移动一帧 进行内部的浮点运动,返回移动的坐标结果
        /// </summary>
        internal Vector2Int Move()
        {
            FPosition += MoveSpeed * Time.DeltaTime;
            int dx = 0, dy = 0;
            if(FPosition.X > 1)
            {
                FPosition.X -= 1;
                dx += 1;
            }
            else if(FPosition.X < -1)
            {
                FPosition.X += 1;
                dx -= 1;
            }

            if (FPosition.Y > 1)
            {
                FPosition.Y -= 1;
                dy += 1;
            }
            else if (FPosition.Y < -1)
            {
                FPosition.Y += 1;
                dy -= 1;
            }

            return new Vector2Int(dx, dy);
        }

    }
}