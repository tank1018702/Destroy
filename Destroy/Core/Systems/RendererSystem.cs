namespace Destroy
{
    using System;
    using System.Collections.Generic;

    public static class RendererSystem
    {
        private static bool ready;
        private static Renderer[,] renderers;
        private static Renderer[,] rendererBuffers;
        private static int minX;
        private static int maxX;
        private static int minY;
        private static int maxY;

        public static void Init(GameObject cameraObj)
        {
            Camera camera = cameraObj.GetComponent<Camera>();
            if (camera != null)
            {
                Transform transform = cameraObj.GetComponent<Transform>();
                int bufferHeight = camera.BufferHeight;
                int bufferWidth = camera.BufferWidth;

                ready = true;
                renderers = new Renderer[bufferHeight, bufferWidth];
                rendererBuffers = new Renderer[bufferHeight, bufferWidth];
                minX = transform.Position.X;
                maxX = transform.Position.X + bufferWidth - 1;
                minY = transform.Position.Y - bufferHeight + 1;
                maxY = transform.Position.Y;
            }
        }

        public static void Update(List<GameObject> gameObjects)
        {
            if (!ready)
            {
                Debug.Error("渲染系统未初始化!");
                return;
            }
            List<GameObject> visibleGameObjects = new List<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer == null)
                    continue;
                Transform transform = gameObject.GetComponent<Transform>();
                //可见性过滤
                if (VisibilityFilter(renderer, transform))
                    visibleGameObjects.Add(gameObject);
            }
            foreach (var gameObject in visibleGameObjects)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                Transform transform = gameObject.GetComponent<Transform>();
                renderers[transform.Position.X, transform.Position.Y] = renderer;
            }
            RendererCanvasBuffer();
        }

        public static bool VisibilityFilter(Renderer renderer, Transform transform)
        {
            int x = transform.Position.X;
            int y = transform.Position.Y;
            if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                return true;
            else
                return false;
        }

        public static void RendererCanvasBuffer()
        {
            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    Renderer renderer = renderers[i, j];
                    Renderer bufferRenderer = rendererBuffers[i, j];
                    //Diff
                    if (renderer != bufferRenderer)
                    {
                        Console.SetCursorPosition(j, i);
                        Print.Draw(renderer.Str, renderer.ForeColor, renderer.BackColor);
                    }
                }
            }
            //Cache
            rendererBuffers = renderers;
        }
    }
}