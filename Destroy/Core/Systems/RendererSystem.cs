namespace Destroy
{
    using System;
    using System.Collections.Generic;

    public static class RendererSystem
    {
        private static RendererData canvas;
        private static RendererData canvasBuffer;

        public static void Init(int width, int height)
        {
            canvas = new RendererData(width, height);
            canvasBuffer = new RendererData(width, height);
        }

        public static void Update(List<GameObject> gameObjects)
        {
            //相机未初始化
            if (canvas.Equals(default(RendererData)))
                return;

            foreach (GameObject gameObject in gameObjects)
            {
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (renderer == null || !renderer.Initialized)
                    continue;

                Transform transform = gameObject.GetComponent<Transform>();
                PreRendererCanvas(renderer.Data, transform);
            }
            RendererCanvasBuffer();
        }

        public static void PreRendererCanvas(RendererData rendererData, Transform transform)
        {
            for (int i = 0; i < rendererData.Height; i++)
            {
                for (int j = 0; j < rendererData.Width; j++)
                {
                    int x = transform.Position.X + j;
                    int y = -1;
                    //算Y轴方向
                    if (transform.Coordinate == CoordinateType.Normal)
                        y = transform.Position.Y - i;
                    else if (transform.Coordinate == CoordinateType.Window)
                        y = transform.Position.Y + i;

                    char c = rendererData.Chars[i, j];
                    ConsoleColor foreColor = rendererData.ForeColors[i, j];
                    ConsoleColor backColor = rendererData.BackColors[i, j];
                    //根据坐标系在画布中设置元素
                    Coordinate.SetInArray(canvas.Chars, c, x, y, transform.Coordinate);
                    Coordinate.SetInArray(canvas.ForeColors, foreColor, x, y, transform.Coordinate);
                    Coordinate.SetInArray(canvas.BackColors, backColor, x, y, transform.Coordinate);
                }
            }
        }

        public static void RendererCanvasBuffer()
        {
            for (int i = 0; i < canvas.Height; i++)
            {
                for (int j = 0; j < canvas.Width; j++)
                {
                    //Diff
                    if (canvas.Chars[i, j] != canvasBuffer.Chars[i, j])
                    {
                        int x = j;
                        int y = canvas.CharWidth * i;
                        Print.SetCursorPos(x, y);
                        char c = canvas.Chars[i, j];
                        ConsoleColor foreColor = canvas.ForeColors[i, j];
                        ConsoleColor backColor = canvas.BackColors[i, j];
                        Print.Draw(c, foreColor, backColor);
                    }
                }
            }
            //Cache
            canvasBuffer = canvas;
        }
    }
}