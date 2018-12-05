namespace Destroy
{
    public class NetworkSpawner : Component
    {
        public NetworkSpawner() { }

        public override Component Clone()
        {
            NetworkSpawner networkSpawner = new NetworkSpawner();
            networkSpawner.Name = Name;
            networkSpawner.Active = Active;
            return networkSpawner;
        }
    }
}