namespace Destroy
{
    using System;

    public abstract class Script : Component
    {
        public bool Started;

        public Script() => Started = false;

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void OnCollision(Collider collision) { }

        public override Component Clone()
        {
            throw new NotImplementedException("不要调用该方法!");
        }
    }
}