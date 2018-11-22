namespace Destroy
{
    using System;
    using System.Collections.Generic;

    public static class RendererSystem
    {
        private static bool ready;
        private static Vector2Int cameraPos;
        private static Matrix world2camera;
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
                ready = true;
                cameraPos = cameraObj.GetComponent<Transform>().Position;
                int bufferHeight = camera.BufferHeight;
                int bufferWidth = camera.BufferWidth;
                world2camera = new Matrix(2, 2);
                world2camera[0, 0] = 0;
                world2camera[0, 1] = 1;
                world2camera[1, 0] = -1;
                world2camera[1, 1] = 0;
                renderers = new Renderer[bufferHeight, bufferWidth];
                rendererBuffers = new Renderer[bufferHeight, bufferWidth];
                minX = cameraPos.X;
                maxX = cameraPos.X + bufferWidth - 1;
                minY = cameraPos.Y - bufferHeight + 1;
                maxY = cameraPos.Y;
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
                int x = transform.Position.X;
                int y = transform.Position.Y;
                if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                    visibleGameObjects.Add(gameObject);
            }
            foreach (var gameObject in visibleGameObjects)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                Transform transform = gameObject.GetComponent<Transform>();

                Vector2Int vector = transform.Position - cameraPos;
                Matrix vec = new Matrix(1, 2);
                vec[0, 0] = vector.X;
                vec[0, 1] = vector.Y;
                Matrix matrix = vec * world2camera; //获得旋转后的坐标系
                Vector2Int tVector = new Vector2Int(matrix[0, 0], matrix[0, 1]);
                renderers[tVector.X, tVector.Y] = renderer;
            }
            RendererCanvasBuffer();
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
                        if (renderer == null)
                            Print.Draw("  ", ConsoleColor.Gray, ConsoleColor.Black);
                        else
                            Print.Draw(renderer.Str, renderer.ForeColor, renderer.BackColor);
                    }
                }
            }
            //Cache
            for (int i = 0; i < renderers.GetLength(0); i++)
                for (int j = 0; j < renderers.GetLength(1); j++)
                    rendererBuffers[i, j] = renderers[i, j];
            //Clean
            renderers = new Renderer[rendererBuffers.GetLength(0), rendererBuffers.GetLength(1)];
        }
    }
}