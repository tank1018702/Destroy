namespace Destroy
{
    public class Collider : Component
    {
        public bool IsTrigger;

        public Collider() => IsTrigger = false;
    }
}