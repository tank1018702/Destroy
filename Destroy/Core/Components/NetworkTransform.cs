namespace Destroy
{
    public class NetworkTransform : Component
    {
        public int SendRate;

        public NetworkTransform() => SendRate = 20;
    }
}