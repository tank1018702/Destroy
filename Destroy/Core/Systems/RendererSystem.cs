using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy
{
    public class RendererSystem
    {
        public static void RenderGameObject(List<GameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.GetComponent<Renderer>() == null ||
                    gameObject.GetComponent<Transform>() == null)
                    continue;


            }
        }
    }
}