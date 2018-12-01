namespace Destroy.Net
{
    public class NetworkTransform : Component
    {
        public NetworkTransform() { }

        public override Component Clone()
        {
            NetworkTransform netTransform = new NetworkTransform();
            netTransform.Name = Name;
            netTransform.Active = Active;
            return netTransform;
        }
    }
}