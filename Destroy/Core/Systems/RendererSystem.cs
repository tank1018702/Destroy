namespace Destroy
{
    using System.Collections.Generic;

    public static class RendererSystem
    {
        public static void Update(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                Transform transform = gameObject.GetComponent<Transform>();
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer == null || transform == null)
                    continue;

            }
        }
    }
}