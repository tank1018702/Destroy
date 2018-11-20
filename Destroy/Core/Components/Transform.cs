namespace Destroy
{
    public class Transform : Component
    {
        public Vector2Int Position;

        public Transform() => Position = Vector2Int.Zero;
    }
}