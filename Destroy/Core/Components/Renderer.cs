namespace Destroy
{
    using System;

    public class Renderer : Component
    {
        /// <summary>
        /// 原数组
        /// </summary>
        public RendererData Data;
        /// <summary>
        /// 是否不进行裁剪
        /// </summary>
        public bool Cut;
        /// <summary>
        /// 裁剪尺寸
        /// </summary>
        public Vector2Int Size;
        /// <summary>
        /// 裁剪坐标
        /// </summary>
        public Vector2Int Position;
        /// <summary>
        /// 裁剪坐标系
        /// </summary>
        public CoordinateType Coordinate;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(RendererData data, bool cut, Vector2Int size, Vector2Int position, CoordinateType coordinate)
        {
            Data = data;
            Cut = cut;
            Size = size;
            Position = position;
            Coordinate = coordinate;
            Initialized = true;
        }

        /// <summary>
        /// 裁剪获得新的数组
        /// </summary>
        public RendererData Get()
        {
            RendererData cut = new RendererData(Size.X, Size.Y);
            for (int i = 0; i < cut.Height; i++)
            {
                for (int j = 0; j < cut.Width; j++)
                {
                    int x = Position.X + j;
                    int y = -1;

                    if (Coordinate == CoordinateType.Normal)
                        y = Position.Y - i;
                    else if (Coordinate == CoordinateType.Window)
                        y = Position.Y + i;

                    char c = global::Destroy.Coordinate.GetInArray(Data.Chars, x, y, Coordinate);
                    ConsoleColor foreColor = global::Destroy.Coordinate.GetInArray(Data.ForeColors, x, y, Coordinate);
                    ConsoleColor backColor = global::Destroy.Coordinate.GetInArray(Data.BackColors, x, y, Coordinate);
                    cut.Chars[i, j] = c;
                    cut.ForeColors[i, j] = foreColor;
                    cut.BackColors[i, j] = backColor;
                }
            }
            return cut;
        }
    }
}