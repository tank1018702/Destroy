namespace Destroy
{
    public abstract class Script : Component
    {
        public abstract void Start();

        public abstract void Update(float deltaTime);
    }
}