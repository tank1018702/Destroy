using System;
using System.Collections.Generic;

namespace ZombieInfection
{
    /// <summary>
    /// 组件标识
    /// </summary>
    public enum ComponentFlag
    {
        Position = 1,
        Velocity = 2,
        Collider = 4,
        Renderer = 8,
        Health = 16,
        PlayerController = 32,
        AIController = 64,
        NetworkId = 128,
        Map = 256,
        Fire = 512,
    }

    /// <summary>
    /// 组件
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// 所属实体的ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
    }

    /// <summary>
    /// 渲染组件
    /// </summary>
    public class Renderer : Component
    {
        /// <summary>
        /// 字符
        /// </summary>
        public string Str { get; set; }

        /// <summary>
        /// 前景色
        /// </summary>
        public ConsoleColor ForeColor { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public ConsoleColor BackColor { get; set; }
    }

    /// <summary>
    /// 位置组件
    /// </summary>
    public class Position : Component
    {
        /// <summary>
        /// 位置坐标
        /// </summary>
        public Vector2 Point { get; set; }

        /// <summary>
        /// 坐标系
        /// </summary>
        public Map Coordinate { get; set; }
    }

    /// <summary>
    /// 速度组件
    /// </summary>
    public class Velocity : Component
    {
        /// <summary>
        /// 每秒速度
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public Vector2 Direction { get; set; }

        /// <summary>
        /// 速度向量 : Speed * Direction
        /// </summary>
        public Vector2 Vector { get; set; }
    }

    /// <summary>
    /// 生命组件
    /// </summary>
    public class Health : Component
    {
        /// <summary>
        /// 生命值
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// 死亡
        /// </summary>
        public bool Dead { get; set; }
    }

    /// <summary>
    /// 碰撞组件
    /// </summary>
    public class Collider : Component
    {
        /// <summary>
        /// 触发器
        /// </summary>
        public bool IsTrigger { get; set; }
    }

    /// <summary>
    /// 角色组件
    /// </summary>
    public class Character : Component
    {
        /// <summary>
        /// 角色类型
        /// </summary>
        public enum Type
        {
            Player,
            AI,
        }

        /// <summary>
        /// 是玩家还是AI
        /// </summary>
        public Type CharType { get; set; }
    }

    /// <summary>
    /// 玩家控制组件
    /// </summary>
    public class PlayerController : Component
    {
        public bool IsFire { get; set; }
        public bool IsSwitchLeft { get; set; }
        public bool IsSwitchRight { get; set; }

        public KeyCode Up { get; set; }
        public KeyCode Down { get; set; }
        public KeyCode Left { get; set; }
        public KeyCode Right { get; set; }

        public KeyCode Fire { get; set; }
        public KeyCode SwitchLeftGun { get; set; }
        public KeyCode SwitchRightGun { get; set; }
    }

    /// <summary>
    /// AI控制器
    /// </summary>
    public class AIController : Component
    {

    }

    /// <summary>
    /// 网络组件
    /// </summary>
    public class NetworkId : Component
    {

    }

    /// <summary>
    /// 地图组件
    /// </summary>
    public class Map : Component
    {
        /// <summary>
        /// 地图长度, Y轴。
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 地图宽度, X轴。
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 原点为屏幕左上角(0, 0), X轴为屏幕的left方向, Y轴为屏幕的up方向。
        /// </summary>
        public Entity[,] NewMapData { get; set; }

        /// <summary>
        /// 原点为屏幕左上角(0, 0), X轴为屏幕的left方向, Y轴为屏幕的up方向。
        /// </summary>
        public Entity[,] OldMapData { get; set; }
    }

    /// <summary>
    /// 火焰组件
    /// </summary>
    public class Fire : Component
    {
        /// <summary>
        /// 每秒伤害
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// 范围
        /// </summary>
        public int Range { get; set; }
    }
}