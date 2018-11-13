using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy
{
    public class Camera : Component
    {
        public Renderer Map;
        public Renderer FOV;
        public Vector2Int Position;
    }
}