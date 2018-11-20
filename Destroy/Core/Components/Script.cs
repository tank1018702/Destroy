namespace Destroy
{
    public abstract class Script : Component
    {
        public bool Started;

        public Script() => Started = false;

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void OnCollisionEnter(Collider collision) { }

        public virtual void OnTriggerEnter(Collider collision) { }
    }
}