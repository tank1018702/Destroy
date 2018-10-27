using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Destroy.ECS
{
    public struct Entity
    {
        public int ID;

        public Entity(int id) => ID = id;
    }
}
