namespace Destroy
{
    public class Collider : Component
    {
        public Vector2Int Size;

        public void Init(Vector2Int size)
        {
            Size = size;
            Initialized = true;
        }
    }
}