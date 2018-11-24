namespace Destroy
{
    public class Transform : Component
    {
        public Vector2Int Position;

        public Transform() => Position = Vector2Int.Zero;

        public void Translate(Vector2Int vector) => Position += vector;

        public override Component Clone()
        {
            Transform transform = new Transform();
            transform.Name = Name;
            transform.Active = Active;
            transform.Position = Position;
            return transform;
        }
    }
}