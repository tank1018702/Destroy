using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZombieInfection
{
    public class Program
    {
        private static void Main()
        {
            EntityFactory factory = new EntityFactory();
            //创建实体
            Entity player = factory.CreatPlayer();
            //添加系统
            Gameplay.Instance.AddSystem(new PlayerControl());
            Gameplay.Instance.AddSystem(new AIControl());
            Gameplay.Instance.AddSystem(new Movement());
            Gameplay.Instance.AddSystem(new Physics());
            Gameplay.Instance.AddSystem(new FireSys());
            Gameplay.Instance.AddSystem(new HealthSys());
            Gameplay.Instance.AddSystem(new Network());
            Gameplay.Instance.AddSystem(new PreRender());
            Gameplay.Instance.AddSystem(new Render());
            //添加实体
            Gameplay.Instance.AddEntity(player);
            Gameplay.Instance.AddEntity(MapSingleTon.Instance);

            //执行
            Gameplay.Instance.Run();

            Console.ReadKey();
        }
    }
}