namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /*
     * 12/7 by Kyasever
     * 新增了复杂渲染系统,暂时使用了清空刷新,当检测到变动时,无条件先输出一行空格清空,然后再输入改动后的内容
     * 之后版本要重新优化
     * TODO: 
     * 新建render类 作为贴图静态数据 renderer是渲染器,用来容纳render
     * 代码比较混乱,需要重构
     */
    public static class RendererSystem
    {
        public static ConsoleColor DefaultColorFore = ConsoleColor.Gray;
        public static ConsoleColor DefaultColorBack = ConsoleColor.Black;
        private static GameObject camera;
        private static Matrix world2camera; //顺时针旋转坐标系90度
        private static int charWidth;
        private static int height;
        private static int width;
        private static Renderer[,] renderers;
        private static Renderer[,] rendererBuffers;
        //可能需要一组staticRenderer 先把static渲染到Renderer中.然后再改动renderer,这样就可以减少一定的渲染工作量


        public static void Init(GameObject camera)
        {
            if (!camera || !camera.Active)
            {
                return;
            }

            Camera cameraComponent = camera.GetComponent<Camera>();
            if (!cameraComponent || !cameraComponent.Active)
            {
                return;
            }

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
            {
                return;
            }

            Camera cam = camera.GetComponent<Camera>();
            if (!cam || !cam.Active)
            {
                return;
            }

            Vector2Int cameraPos = camera.GetComponent<Transform>().Position;
            int minX = cameraPos.X;
            int maxX = cameraPos.X + width - 1;
            int minY = cameraPos.Y - height + 1;
            int maxY = cameraPos.Y;

            List<KeyValuePair<uint, object>> pairs = new List<KeyValuePair<uint, object>>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.Active)
                {
                    continue;
                }

                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (!renderer || !renderer.Active)
                {
                    continue;
                }

                Transform transform = gameObject.GetComponent<Transform>();
                if (IsInCamera(transform.Position))
                {
                    pairs.Add(new KeyValuePair<uint, object>(renderer.Depth, renderer));
                }



            }
            //pairs.Sort();
            Mathematics.QuickSort(pairs);
            pairs.Reverse(); //排序(从大到小)

            foreach (KeyValuePair<uint, object> pair in pairs)
            {
                Renderer renderer = (Renderer)pair.Value;

                //如果是组渲染器,那么包含一系列的点.如果是点渲染器或者字符串渲染器,则只包含一个点
                if (renderer.GetType() == typeof(GroupRenderer))
                {
                    GroupRenderer gr = renderer as GroupRenderer;
                    foreach (KeyValuePair<Renderer, Vector2Int> v in gr.list)
                    {
                        Transform transform = renderer.GetComponent<Transform>();
                        Vector2Int vector = transform.Position + v.Value - cameraPos;
                        vector *= world2camera;     //获得该点在摄像机坐标系中的位置
                        if (vector.X > 0 && vector.X < width && vector.Y > 0 && vector.Y < height)
                            renderers[vector.X, vector.Y] = v.Key;
                    }
                }
                else
                {
                    Transform transform = renderer.GetComponent<Transform>();

                    Vector2Int vector = transform.Position - cameraPos;
                    vector *= world2camera;     //获得该点在摄像机坐标系中的位置
                    renderers[vector.X, vector.Y] = renderer;
                }


            }

            DisplayGameObjects();
        }

        /// <summary>
        /// 判断是否位于摄像机内,待定
        /// </summary>
        private static bool IsInCamera(Vector2Int pos)
        {
            Vector2Int cameraPos = camera.GetComponent<Transform>().Position;
            int minX = cameraPos.X;
            int maxX = cameraPos.X + width - 1;
            int minY = cameraPos.Y - height + 1;
            int maxY = cameraPos.Y;
            //可见性过滤
            int x = pos.X;
            int y = pos.Y;
            if (x >= minX && x <= maxX && y >= minY && y <= maxY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void DisplayGameObjects()
        {
            //先写入字符串渲染器移动产生的空格.
            for (int i = 0; i < renderers.GetLength(0); i++)
            {
                for (int j = 0; j < renderers.GetLength(1); j++)
                {
                    Renderer renderer = renderers[i, j];
                    Renderer bufferRenderer = rendererBuffers[i, j];
                    //Diff
                    //Diff
                    if (renderer != bufferRenderer)
                    {
                        Console.SetCursorPosition(j * charWidth, i);
                        if (bufferRenderer != null)
                        {
                            if (bufferRenderer.GetType() == typeof(PosRenderer))
                            {
                                //如果是点贴图,插入一个字符宽度的空格
                                StringBuilder builder = new StringBuilder();
                                for (int k = 0; k < charWidth; k++)
                                {
                                    builder.Append(" ");
                                }

                                string space = builder.ToString();
                                Print.Draw(space, DefaultColorFore, DefaultColorBack);
                            }
                            else if (bufferRenderer.GetType() == typeof(StringRenderer))
                            {
                                int length = ((StringRenderer)bufferRenderer).length;

                                //i表示是横排的第j个格子,总数量为renderers.GetLength(1)
                                //表示剩余几个格子到头
                                int releaseLength = (renderers.GetLength(1) - j) * charWidth;

                                //如果是字符串贴图,插入一个字符宽度的空格
                                StringBuilder builder = new StringBuilder();
                                for (int k = 0; k < Math.Min(length, releaseLength); k++)
                                {
                                    builder.Append(" ");
                                }
                                string space = builder.ToString();
                                Print.Draw(space, DefaultColorFore, DefaultColorBack);
                            }
                        }
                    }
                }
            }
            //再写入新的字符串对象
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
                        if (renderer != null)
                        {
                            if (renderer.GetType() == typeof(PosRenderer))
                            {
                                Print.Draw(renderer.GetStr(), renderer.ForeColor, renderer.BackColor);
                            }
                            else if (renderer.GetType() == typeof(StringRenderer))
                            {
                                int length = ((StringRenderer)renderer).length;
                                //表示剩余几个格子到头
                                int releaseLength = (renderers.GetLength(1) - j) * charWidth;
                                if (length <= releaseLength)
                                {
                                    Print.Draw(renderer.GetStr(), renderer.ForeColor, renderer.BackColor);
                                }
                                else
                                {
                                    StringBuilder builder = new StringBuilder();
                                    int sum = 0;
                                    foreach (char v in renderer.GetStr())
                                    {
                                        sum += Print.CharWide(v);
                                        //强制截断长度一个标准Renderer单位的字符
                                        if (sum > releaseLength)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            builder.Append(v);
                                        }
                                    }
                                    Print.Draw(builder.ToString(), renderer.ForeColor, renderer.BackColor);
                                }
                            }
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