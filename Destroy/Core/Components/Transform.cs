namespace Destroy
{
    public class Transform : Component
    {
        public Vector2Int Position { get; set; }

        public CoordinateType Coordinate { get; set; }

        public Transform()
        {
            Position = Vector2Int.Zero;
            Coordinate = CoordinateType.Window;
        }
    }
}