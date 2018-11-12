namespace Destroy
{
    using System.Collections.Generic;

    public abstract class Collider : Component
    {
        public Vector2Int Pos;
        public Vector2Int Size;

        public bool Intersects(Collider collider)
        {
            if (Pos.X >= collider.Pos.X + collider.Size.X || collider.Pos.X >= Pos.X + Size.X)
            {
                return false;
            }

            if (Pos.Y >= collider.Pos.Y + collider.Size.Y || collider.Pos.Y >= Pos.Y + Size.Y)
            {
                return false;
            }

            return true;
        }
    }
}