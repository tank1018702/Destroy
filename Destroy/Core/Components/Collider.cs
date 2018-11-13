namespace Destroy
{
    public class Collider : Component
    {
        public Vector2Int Size;

        public Collider()
        {
            Size = new Vector2Int(1, 1);
        }

        public Collider(Vector2Int size)
        {
            Size = size;
        }
    }
}