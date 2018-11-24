namespace Destroy
{
    using System;
    using System.Collections.Generic;

    public static class RendererSystem
    {
        private static GameObject camera;
        private static Matrix world2camera;
        private static int bufferHeight;
        private static int bufferWidth;
        private static Renderer[,] renderers;
        private static Renderer[,] rendererBuffers;

        public static void Init(GameObject camera)
        {
            Camera c = camera.GetComponent<Camera>();
            bufferHeight = c.BufferHeight;
            bufferWidth = c.BufferWidth;
            renderers = new Renderer[bufferHeight, bufferWidth];
            rendererBuffers = new Renderer[bufferHeight, bufferWidth];

            world2camera = new Matrix(2, 2); //顺时针旋转点90度
            world2camera[0, 0] = 0;
            world2camera[0, 1] = -1;
            world2camera[1, 0] = 1;
            world2camera[1, 1] = 0;
            world2camera *= -1; //旋转点变为旋转坐标系

            RendererSystem.camera = camera;
        }

        public static void Update(List<GameObject> gameObjects)
        {
            if (!camera || !camera.GetComponent<Camera>())
            {
                Debug.Error("渲染系统未初始化!");
                return;
            }
            Vector2Int cameraPos = camera.GetComponent<Transform>().Position;
            int minX = cameraPos.X;
            int maxX = cameraPos.X + bufferWidth - 1;
            int minY = cameraPos.Y - bufferHeight + 1;
            int maxY = cameraPos.Y;

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
                vector *= world2camera;     //获得该点在摄像机坐标系中的位置
                renderers[vector.X, vector.Y] = renderer;
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