namespace Destroy
{
    using System.Collections.Generic;

    public static class RendererSystem
    {
        public static void Update(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                Transform transform = gameObject.GetComponent<Transform>();
                if (renderer == null || transform == null)
                    continue;

                Graphics.Block block = new Graphics.Block
                (
                    renderer.Chars,
                    renderer.CharWidth,
                    transform.Coordinate,
                    transform.Position,
                    renderer.ForeColors,
                    renderer.BackColors
                );
                Graphics.RendererSystem.RenderBlock(block);
            }
        }
    }
}