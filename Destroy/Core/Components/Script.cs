namespace Destroy
{
    public abstract class Script : Component
    {
        public bool Started;

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void OnCollision(Collider collision) { }
    }
}