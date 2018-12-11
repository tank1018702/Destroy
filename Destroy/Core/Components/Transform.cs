namespace Destroy
{
    public class Transform : Component, IPersistent
    {
        public Vector2Int Position;

        public Transform()=> Position = Vector2Int.Zero;

        public void Translate(Vector2Int vector) => Position += vector;
    }
}