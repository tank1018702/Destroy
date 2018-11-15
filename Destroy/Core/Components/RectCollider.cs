namespace Destroy
{
    public class RectCollider : Collider
    {
        public Vector2Int Size;

        public void Init(Vector2Int size)
        {
            Size = size;
            Initialized = true;
        }
    }
}