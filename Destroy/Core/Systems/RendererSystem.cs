﻿namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static class RendererSystem
    {
        private static GameObject camera;
        private static Matrix world2camera;
        private static int bufferHeight;
        private static int bufferWidth;
        private static Renderer[,] renderers;
        private static Renderer[,] rendererBuffers;

        public static void Init(GameObject camera)
        {
            Camera cameraComponent = camera.GetComponent<Camera>();
            bufferHeight = cameraComponent.BufferHeight;
            bufferWidth = cameraComponent.BufferWidth;
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
                return;
            Vector2Int cameraPos = camera.GetComponent<Transform>().Position;
            int minX = cameraPos.X;
            int maxX = cameraPos.X + bufferWidth - 1;
            int minY = cameraPos.Y - bufferHeight + 1;
            int maxY = cameraPos.Y;

            List<KeyValuePair<uint, object>> pairs = new List<KeyValuePair<uint, object>>();

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
                    pairs.Add(new KeyValuePair<uint, object>(renderer.Order, gameObject));
            }
            Mathematics.InsertionSort(pairs);
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
            Display();
        }
        
        public static void Display()
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
                        //renderer移动
                        if (renderer == null && bufferRenderer != null)
                        {
                            StringBuilder builder = new StringBuilder();
                            for (int k = 0; k < bufferRenderer.Width; k++)
                                builder.Append(" ");
                            string space = builder.ToString();

                            int x = j * bufferRenderer.Width;
                            if (x > Console.BufferWidth - 1)
                                x = Console.BufferWidth - 1;
                            int y = i;

                            Console.SetCursorPosition(x, y);
                            Print.Draw(space, ConsoleColor.Gray, ConsoleColor.Black);
                        }
                        //renderer更新
                        else
                        {
                            int x = j * renderer.Width;
                            if (x > Console.BufferWidth - 1)
                                x = Console.BufferWidth - 1;
                            int y = i;

                            Console.SetCursorPosition(x, y);
                            Print.Draw(renderer.Str, renderer.ForeColor, renderer.BackColor);
                        }
                    }
                }
            }
            //Cache
            for (int i = 0; i < renderers.GetLength(0); i++)
                for (int j = 0; j < renderers.GetLength(1); j++)
                    rendererBuffers[i, j] = renderers[i, j];
            //Clear
            renderers = new Renderer[rendererBuffers.GetLength(0), rendererBuffers.GetLength(1)];
        }
    }
}