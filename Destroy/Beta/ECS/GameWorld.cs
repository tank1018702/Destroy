using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy.ECS
{
    public class GameWorld
    {
        public static EntityManager manager;

        public static EntityManager CreatOrGetManager()
        {
            if (manager == null)
                manager = new EntityManager();

            return manager;
        }
    }
}