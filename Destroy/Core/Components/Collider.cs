namespace Destroy
{
    public class Collider : Component
    {
        public Collider() { }

        public override Component Clone()
        {
            Collider collider = new Collider();
            collider.Name = Name;
            collider.Active = Active;
            return collider;
        }
    }
}