using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy.ECS
{
    abstract class System
    {


        protected abstract void OnUpdate();
    }
}