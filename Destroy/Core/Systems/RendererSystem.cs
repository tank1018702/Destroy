namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class RendererSystem
    {
        private static GameObject camera;
        private static Matrix world2camera; //顺时针旋转坐标系90度
        private static int charWidth;
        private static int height;
        private static int width;
        private static Renderer[,] renderers;
        private static Renderer[,] rendererBuffers;

        public static void Init(GameObject camera)
        {
            if (!camera || !camera.Active)
                return;
            Camera cameraComponent = camera.GetComponent<Camera>();
            if (!cameraComponent || !cameraComponent.Active)
                return;

            RendererSystem.camera = camera;

            world2camera = new Matrix(2, 2); //顺时针旋转点90度
            world2camera[0, 0] = 0;
            world2camera[0, 1] = -1;
            world2camera[1, 0] = 1;
            world2camera[1, 1] = 0;
            world2camera *= -1; //旋转点变为旋转坐标系

            charWidth = cameraComponent.CharWidth;
            height = cameraComponent.Height;
            width = cameraComponent.Width;
            renderers = new Renderer[height, width];
            rendererBuffers = new Renderer[height, width];

            //设置窗口(最右边留出一列)
            Console.CursorVisible = false;
            Console.WindowHeight = height;
            Console.BufferHeight = height;
            Console.WindowWidth = (width + 1) * charWidth;
            Console.BufferWidth = (width + 1) * charWidth;
        }

        internal static void Update(List<GameObject> gameObjects)
        {
            if (!camera || !camera.Active)
                return;
            Camera cam = camera.GetComponent<Camera>();
            if (!cam || !cam.Active)
                return;

            Vector2Int cameraPos = camera.GetComponent<Transform>().Position;
            int minX = cameraPos.X;
            int maxX = cameraPos.X + width - 1;
            int minY = cameraPos.Y - height + 1;
            int maxY = cameraPos.Y;

            List<KeyValuePair<uint, object>> pairs = new List<KeyValuePair<uint, object>>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.Active)
                    continue;
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (!renderer || !renderer.Active)
                    continue;

                Transform transform = gameObject.GetComponent<Transform>();

                //可见性过滤
                int x = transform.Position.X;
                int y = transform.Position.Y;
                if (x >= minX && x <= maxX && y >= minY && y <= maxY)
                    pairs.Add(new KeyValuePair<uint, object>(renderer.Depth, gameObject));
            }
            Mathematics.QuickSort(pairs);
            pairs.Reverse(); //排序(从大到小)
            foreach (var pair in pairs)
            {
                GameObject gameObject = (GameObject)pair.Value;

                Renderer renderer = gameObject.GetComponent<Renderer>();
                Transform transform = gameObject.GetComponent<Transform>();

                Vector2Int vector = transform.Position - cameraPos;
                vector *= world2camera;     //获得该点在摄像机坐标系中的位置
                renderers[vector.X, vector.Y] = renderer;
            }
            DisplayGameObjects();
        }

        private static void DisplayGameObjects()
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
                        Console.SetCursorPosition(j * charWidth, i);
                        //renderer移动
                        if (renderer == null && bufferRenderer != null)
                        {
                            StringBuilder builder = new StringBuilder();
                            for (int k = 0; k < charWidth; k++)
                                builder.Append(" ");
                            string space = builder.ToString();
                            Print.Draw(space, ConsoleColor.Gray, ConsoleColor.Black);
                        }
                        //renderer更新
                        else
                        {
                            Print.Draw(renderer.Str, renderer.ForeColor, renderer.BackColor);
                        }
                    }
                }
            }
            //Cache & //Clear
            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    rendererBuffers[i, j] = renderers[i, j];
                    renderers[i, j] = null;
                }
            }
        }
    }
}