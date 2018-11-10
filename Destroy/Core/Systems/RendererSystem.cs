namespace Destroy
{
    using System.Collections.Generic;

    public static class RendererSystem
    {
        public static int GameObjectCount => gameObjects.Count;

        private static List<GameObject> gameObjects;

        public static void Update(List<GameObject> gameObjects)
        {
            RendererSystem.gameObjects = gameObjects;

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.GetComponent<Renderer>() == null ||
                    gameObject.GetComponent<Transform>() == null)
                    continue;

            }
        }
    }
}