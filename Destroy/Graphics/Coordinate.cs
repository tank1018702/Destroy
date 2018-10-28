namespace Destroy.Graphics
{
    public class Coordinate
    {
        public int Height { get; private set; }

        public enum Mode
        {
            RightX_UpY,
            RightX_DownY,
        }

        public Mode Type { get; private set; }

        public Coordinate(Mode type, int height)
        {
            Type = type;
            Height = height;
        }
    }
}