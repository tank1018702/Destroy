using System;
using System.Collections.Generic;

namespace ZombieInfection
{
    public class MapSingleTon
    {
        private static Entity instance { get; set; }

        public static Entity Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Gameplay.Instance.CreatEmptyEntity();
                    Map map = instance.AddComponent<Map>();
                    Position position = instance.AddComponent<Position>();

                    position.Point = Vector2.Zero;

                    map.Width = 20;
                    map.Length = 20;
                    map.NewMapData = new Entity[map.Width, map.Length];
                    map.OldMapData = new Entity[map.Width, map.Length];

                    //初始化地图边
                    for (int i = 0; i < map.Width; i++)
                    {
                        Entity entity = Gameplay.Instance.CreatEmptyEntity();

                        Position pos = entity.AddComponent<Position>();
                        Collider collider = entity.AddComponent<Collider>();
                        Renderer renderer = entity.AddComponent<Renderer>();

                        pos.Coordinate = map;
                        pos.Point = new Vector2(i, 0);

                        collider.IsTrigger = false;
                        renderer.Str = "墙";
                        renderer.ForeColor = ConsoleColor.Blue;
                        renderer.BackColor = ConsoleColor.Yellow;
                        //加载进游戏
                        Gameplay.Instance.AddEntity(entity);
                    }
                }
                return instance;
            }
        }

        public static List<Entity> GetAround(Map map, Vector2Int pos, int dis)
        {
            List<Entity> entities = new List<Entity>();
            for (int x = -dis; x < dis + 1; x++)
            {
                //限制x的取值范围
                if (x < 0)
                    x = 0;
                if (x > map.Width - 1)
                    x = map.Width - 1;
                for (int y = -dis; y < dis + 1; y++)
                {
                    //限制y的取值范围
                    if (y < 0)
                        y = 0;
                    if (y > map.Length - 1)
                        y = map.Length - 1;
                    Entity entity = map.OldMapData[x, y];

                    if (entity != null && !entities.Contains(entity))
                        entities.Add(entity);
                }
            }

            return entities;
        }
    }
}