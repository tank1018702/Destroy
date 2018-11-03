using System.Collections.Generic;
using System.Threading;

namespace ZombieInfection
{
    public class Gameplay
    {
        private int id;
        private static Gameplay instance;

        /// <summary>
        /// 自增Id
        /// </summary>
        private int Id { get { id++; return id; } }

        /// <summary>
        /// 实体 [ID 实体]
        /// </summary>
        private Dictionary<int, Entity> Entities { get; set; }

        /// <summary>
        /// 系统 [Name 系统]
        /// </summary>
        private Dictionary<string, System> Systems { get; set; }

        /// <summary>
        /// 每秒帧数
        /// </summary>
        private int FPS { get => 20; }

        /// <summary>
        /// 休眠时间
        /// </summary>
        private int DelayTime
        {
            get
            {
                int time = 1000 / FPS;
                if (time < 1)
                    time = 1;
                return time;
            }
        }

        private Gameplay()
        {
            Entities = new Dictionary<int, Entity>();
            Systems = new Dictionary<string, System>();
        }

        private void Update()
        {
            foreach (var system in Systems.Values)
                system.Update();
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static Gameplay Instance
        {
            get
            {
                if (instance == null)
                    instance = new Gameplay();
                return instance;
            }
        }

        /// <summary>
        /// 每帧间隔时间
        /// </summary>
        public float DeltaTime
        {
            get => (float)1 / FPS;
        }

        /// <summary>
        /// 筛选实体
        /// </summary>
        public void EntityFilter(Entity entity)
        {
            //遍历所有系统过滤玩家
            foreach (var system in Systems.Values)
                system.EntityFilter(entity);
        }

        /// <summary>
        /// 创建空实体
        /// </summary>
        public Entity CreatEmptyEntity()
        {
            Entity entity = new Entity(Id);
            return entity;
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        public void AddEntity(Entity entity)
        {
            //不能注册相同实体
            if (Entities.ContainsKey(entity.ID))
                return;
            Entities.Add(entity.ID, entity);
            //筛选一次
            EntityFilter(entity);
        }

        /// <summary>
        /// 添加系统
        /// </summary>
        public void AddSystem(System system)
        {
            //不能注册相同系统
            string name = system.GetType().Name;
            if (Systems.ContainsKey(name))
                return;
            Systems.Add(name, system);
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            //遍历筛选所有的实体
            foreach (var entity in Entities)
                EntityFilter(entity.Value);

            while (true)
            {
                Update();
                Thread.Sleep(DelayTime);
            }
        }
    }
}