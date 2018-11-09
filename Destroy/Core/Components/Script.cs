namespace Destroy
{
    public abstract class Script : Component
    {
        public bool Started { get; set; }

        public virtual void Start() { }

        public virtual void Update() { }
    }
}