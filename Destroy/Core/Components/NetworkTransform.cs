namespace Destroy
{
    public class NetworkTransform : Component
    {
        public int SendRate;

        public NetworkTransform()
        {
            SendRate = 20;
        }

        public override Component Clone()
        {
            NetworkTransform netTransform = new NetworkTransform();
            netTransform.Name = Name;
            netTransform.Active = Active;
            netTransform.SendRate = SendRate;
            return netTransform;
        }
    }
}