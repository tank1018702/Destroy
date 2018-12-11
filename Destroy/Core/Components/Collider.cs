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

    /// <summary>
    /// Mesh组件 默认生成单点Mesh
    /// </summary>
    public class Mesh : Component
    {
        //使用一个标识符标识这个是不是一个单点Mesh,从而进行优化.
        public bool isSingleMesh = true;
        internal List<Vector2Int> posList { get; private set; }
        public Mesh()
        {
            //默认生成一个单点Mesh,只包含原点
            isSingleMesh = true;
            posList = new List<Vector2Int>();
            posList.Add(new Vector2Int(0, 0));
        }

        public void Sort()
        {
            //使用HashSet去重复. 之后排序
            HashSet<Vector2Int> set = new HashSet<Vector2Int>();
            foreach (var v in posList)
            {
                set.Add(v);
            }
            List<Vector2Int> newList = new List<Vector2Int>();
            foreach (var v in set)
            {
                newList.Add(v);
            }
            newList.Sort();
            posList = newList;
        }
        /// <summary>
        /// 进行多点初始化
        /// </summary>
        public bool Init(List<Vector2Int> list)
        {
            //如果只有一个点,相当于没指定
            if (list.Count == 1)
            {
                if (list[0] == Vector2Int.Zero)
                {
                    isSingleMesh = true;
                    posList = list;
                    Debug.Warning("单点Mesh不需要执行Init操作");
                    return true;
                }
                else
                {
                    Debug.Error("初始化失败,锚点必须包含原点");
                    return false;
                }
            }
            //检测是否包含原点,如果包含那么初始化成功
            foreach (var v in list)
            {
                if (v == Vector2Int.Zero)
                {
                    //如果有零点
                    isSingleMesh = false;
                    posList = list;
                    //更改Mesh的时候进行重新排序
                    Sort();
                    return true;
                }
            }
            Debug.Error("初始化失败,锚点必须包含原点");
            return false;
        }
    }

    public abstract class Collider : Component
    {

    }

    /// <summary>
    /// 碰撞体组件,一般来说默认按着Mesh来
    /// </summary>
    public class MeshCollider : Collider
    {
        public List<Vector2Int> ColliderList { get; private set; }
        public MeshCollider()
        {

        }
        public void Init()
        {
            Mesh mesh = GetComponent<Mesh>();
            if (mesh == null)
            {
                mesh = AddComponent<Mesh>();
                Debug.Warning("没有Mesh组件,自动生成一个点Mesh");
            }
            //将对象复制到此组件内. 更改Mesh中的list同样会更改这个
            ColliderList = mesh.posList;
            return;
        }

        public void Change(List<Vector2Int> list)
        {
            Debug.Warning("将修改Collider,使其与Mesh不等,此操作不保证稳定性");
            ColliderList = list;
        }
    }

    public class StaticCollider : Collider
    {
        public List<Vector2Int> ColliderList { get; private set; }
        public StaticCollider()
        {
            Mesh mesh = GetComponent<Mesh>();
            if (mesh == null)
            {
                mesh = AddComponent<Mesh>();
                Debug.Warning("没有Mesh组件,自动生成一个点Mesh");
            }
            //将对象复制到此组件内. 更改Mesh中的list同样会更改这个
            ColliderList = mesh.posList;
            return;
        }

        public void Change(List<Vector2Int> list)
        {
            Debug.Warning("将修改Collider,使其与Mesh不等,此操作不保证稳定性");
            ColliderList = list;
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
            if (FPosition.X > 1)
            {
                FPosition.X -= 1;
                dx += 1;
            }
            else if (FPosition.X < -1)
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