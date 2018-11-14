namespace Destroy
{
    using System.Collections.Generic;

    public static class RendererSystem
    {
        private static RendererData canvas;

        public static void Init(RendererData canvas)
        {
            RendererSystem.canvas = canvas;
        }

        public static void Update(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                Transform transform = gameObject.GetComponent<Transform>();
                if (renderer == null || transform == null)
                    continue;
                if (!renderer.Initialized)
                    continue;
                
            }
        }
    }
}