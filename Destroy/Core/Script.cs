namespace Destroy
{
    public abstract class Script
    {
        public GameObject GameObject;

        public abstract void Start();

        public abstract void Update(float deltaTime);
    }
}