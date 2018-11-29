using System;
using System.Collections.Generic;

namespace ZombieInfection
{
    /// <summary>
    /// 系统
    /// </summary>
    public abstract class System
    {
        /// <summary>
        /// 关注的实体
        /// </summary>
        protected List<Entity> Entities { get; set; }

        public System() => Entities = new List<Entity>();

        /// <summary>
        /// 筛选实体
        /// </summary>
        public abstract void EntityFilter(Entity entity);

        /// <summary>
        /// 每帧更新
        /// </summary>
        public abstract void Update();
    }

    /// <summary>
    /// 渲染系统
    /// </summary>
    public class Render : System
    {
        public Render() => Console.CursorVisible = false;

        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            //拥有坐标 并拥有 Map
            if (entity.HasComponent(ComponentFlag.Position) &&
                entity.HasComponent(ComponentFlag.Map))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {
                Position position = entity.GetComponent<Position>();
                //绘制时取整
                Vector2Int point = (Vector2Int)position.Point;

                Map map = entity.GetComponent<Map>();
                //Diff画图
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Length; y++)
                    {
                        //如果有地图上的实体变动就更新
                        if (map.NewMapData[x, y] != map.OldMapData[x, y])
                        {
                            Entity curEntity = map.NewMapData[x, y];
                            //设置绘制光标
                            Console.SetCursorPosition((point.X + x) * 2, point.Y + y);

                            //有实体就进行绘制
                            if (curEntity != null)
                            {
                                Renderer renderer = curEntity.GetComponent<Renderer>();
                                if (renderer != null)
                                {
                                    Print.Draw(renderer.Str, renderer.ForeColor, renderer.BackColor);
                                }
                            }
                            //没有实体就画空
                            else
                            {
                                Print.Draw("  ", default(ConsoleColor), default(ConsoleColor));
                            }
                        }
                    }
                }
                //缓存
                map.OldMapData = map.NewMapData;
                map.NewMapData = new Entity[map.Width, map.Length];
            }
        }
    }

    /// <summary>
    /// 预渲染系统
    /// </summary>
    public class PreRender : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            if (entity.HasComponent(ComponentFlag.Renderer) &&
                entity.HasComponent(ComponentFlag.Position))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {
                Position position = entity.GetComponent<Position>();
                Renderer renderer = entity.GetComponent<Renderer>();

                position.Point.ToInt(out int x, out int y);
                //赋值
                position.Coordinate.NewMapData[x, y] = entity;
            }
        }
    }

    /// <summary>
    /// 生命值系统
    /// </summary>
    public class HealthSys : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            if (entity.HasComponent(ComponentFlag.Health))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {
                Health health = entity.GetComponent<Health>();
                //判定死亡
                if (health.Value <= 0)
                {
                    health.Dead = true;
                }
            }
        }
    }

    /// <summary>
    /// 火焰系统
    /// </summary>
    public class FireSys : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            if (entity.HasComponent(ComponentFlag.Position) &&
                entity.HasComponent(ComponentFlag.Fire))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {
                Position position = entity.GetComponent<Position>();
                Fire fire = entity.GetComponent<Fire>();

                //获取地图中指定距离内的实体
                List<Entity> entities = MapSingleTon.GetAround(position.Coordinate, (Vector2Int)position.Point, fire.Range);
                foreach (var each in entities)
                {
                    //如果该实体拥有生命组件
                    if (each.HasComponent(ComponentFlag.Health))
                    {
                        Health health = each.GetComponent<Health>();
                        health.Value -= fire.Damage * Gameplay.Instance.DeltaTime; //造成伤害
                    }
                }
            }
        }
    }

    /// <summary>
    /// 物理系统
    /// </summary>
    public class Physics : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            if (entity.HasComponent(ComponentFlag.Collider) &&
                entity.HasComponent(ComponentFlag.Position) &&
                entity.HasComponent(ComponentFlag.Velocity))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {
                Velocity velocity = entity.GetComponent<Velocity>();
                if (velocity.Vector == Vector2.Zero)
                    continue;

                //移动后的点
                Position curPos = entity.GetComponent<Position>();
                //移动前的点
                Vector2 lastPos = curPos.Point - velocity.Vector;
                lastPos.ToInt(out int x, out int y);
                //移动前的点加上方向
                x += (int)velocity.Direction.X;
                y += (int)velocity.Direction.Y;

                //边界范围
                int boardLength, boardWidth;
                if (curPos.Coordinate != null)
                {
                    boardLength = curPos.Coordinate.Length;
                    boardWidth = curPos.Coordinate.Width;
                }
                else
                {
                    boardLength = Console.BufferHeight;
                    boardWidth = Console.BufferWidth;
                }

                //碰撞检测(检测下一格)
                if (x < 0)
                    x = 0;
                if (x > boardWidth - 1)
                    x = boardWidth - 1;
                if (y < 0)
                    y = 0;
                if (y > boardLength - 1)
                    y = boardLength - 1;
                //从上一帧判断
                Entity other = curPos.Coordinate.OldMapData[x, y];
                //有实体
                if (other != null)
                {
                    Collider collider = other.GetComponent<Collider>();
                    //如果有碰撞组件并且不是触发器
                    if (collider != null && !collider.IsTrigger)
                    {
                        curPos.Point = lastPos; //撤销移动
                    }
                }

                //检测地图边界(即使地图中没有其他碰撞体也可以保证不越界)
                float xCast = curPos.Point.X, yCast = curPos.Point.Y;
                if (curPos.Point.X < 0f)
                    xCast = 0f;
                if (curPos.Point.X > boardWidth - 1f)
                    xCast = boardWidth - 1;
                if (curPos.Point.Y < 0f)
                    yCast = 0f;
                if (curPos.Point.Y > boardLength - 1)
                    yCast = boardLength - 1f;
                //防止越界
                curPos.Point = new Vector2(xCast, yCast);
            }
        }
    }

    /// <summary>
    /// 移动系统
    /// </summary>
    public class Movement : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            if (entity.HasComponent(ComponentFlag.Position) &&
                entity.HasComponent(ComponentFlag.Velocity))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {
                //设置速度向量
                Velocity velocity = entity.GetComponent<Velocity>();
                velocity.Vector = velocity.Direction.Normalized * velocity.Speed * Gameplay.Instance.DeltaTime;
                //移动:更改坐标
                Position position = entity.GetComponent<Position>();
                position.Point += velocity.Vector;
            }
        }
    }

    /// <summary>
    /// 角色控制系统
    /// </summary>
    public class PlayerControl : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            if (entity.HasComponent(ComponentFlag.Velocity) &&
                entity.HasComponent(ComponentFlag.PlayerController))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {
                //获取每个玩家的输入
                GetPlayerInput(entity);
            }
        }

        private void GetPlayerInput(Entity entity)
        {
            Velocity velocity = entity.GetComponent<Velocity>();
            PlayerController controller = entity.GetComponent<PlayerController>();

            int x = Input.GetDirectInput(controller.Left, controller.Right);
            int y = Input.GetDirectInput(controller.Down, controller.Up);
            //速度向量:把Y值取反:控制台的Y轴正方向朝下
            velocity.Direction = new Vector2(x, -y);                                //输入向量

            controller.IsFire = Input.GetKey(controller.Fire);                      //开火
            controller.IsSwitchLeft = Input.GetKeyDown(controller.SwitchLeftGun);   //向左切枪
            controller.IsSwitchRight = Input.GetKeyDown(controller.SwitchRightGun); //向右切枪
        }
    }

    /// <summary>
    /// AI控制系统
    /// </summary>
    public class AIControl : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;
            if (entity.HasComponent(ComponentFlag.Velocity) &&
                entity.HasComponent(ComponentFlag.AIController))
                Entities.Add(entity);
            else
                Entities.Remove(entity);
        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {

            }
        }
    }

    /// <summary>
    /// 网络系统
    /// </summary>
    public class Network : System
    {
        public override void EntityFilter(Entity entity)
        {
            if (Entities.Contains(entity))
                return;

        }

        public override void Update()
        {
            foreach (var entity in Entities)
            {

            }
        }
    }
}