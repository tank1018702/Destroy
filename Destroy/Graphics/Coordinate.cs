namespace Destroy.Graphics
{
    public class Coordinate
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public enum Mode
        {
            RightX__UpY,
            RightX__DownY,
        }

        public Mode Type { get; private set; }

        public Coordinate(Mode type, int width, int height)
        {
            Type = type;
            Width = width;
            Height = height;
        }
    }
}