namespace Destroy.Net
{
    public class NetTransform : Component
    {
        public Position Position;

        public NetTransform() => Position = new Position();

        public override Component Clone()
        {
            NetTransform netTransform = new NetTransform();
            netTransform.Name = Name;
            netTransform.Active = Active;
            netTransform.Position = Position;
            return netTransform;
        }
    }
}