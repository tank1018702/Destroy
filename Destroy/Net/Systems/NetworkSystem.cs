namespace Destroy.Net
{
    using System.Collections.Generic;

    internal static class NetworkSystem
    {
        public static void Init(NetworkRole role)
        {
            switch (role)
            {
                case NetworkRole.Client:
                    {

                    }
                    break;
                case NetworkRole.Server:
                    {

                    }
                    break;
                case NetworkRole.Host:
                    {

                    }
                    break;
            }
        }

        public static void Update(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.Active)
                    continue;
                NetTransform netTransform = gameObject.GetComponent<NetTransform>();
                if (!netTransform || !netTransform.Active)
                    continue;

                Transform transform = gameObject.GetComponent<Transform>();
                netTransform.Position = new Position(transform.Position.X, transform.Position.Y);

                byte[] data = Destroy.Serializer.NetSerialize(netTransform);

            }
        }
    }
}