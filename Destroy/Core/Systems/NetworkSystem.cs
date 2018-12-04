namespace Destroy
{
    using System.Collections.Generic;

    public static class NetworkSystem
    {
        public static NetworkRole Role
        {
            get
            {
                if (netClient != null && netServer != null)
                    return NetworkRole.Host;
                else if (netClient != null)
                    return NetworkRole.Client;
                else if (netServer != null)
                    return NetworkRole.Server;
                else
                    return NetworkRole.None;
            }
        }

        private static NetworkServer netServer;

        private static NetworkClient netClient;

        private static List<UDPService> udps;

        public static void Init(NetworkServer server, NetworkClient client, List<UDPService> udps)
        {
            netServer = server;
            netClient = client;
            NetworkSystem.udps = udps;

            netServer?.Start();
            netClient?.Start();
        }

        public static void Update(List<GameObject> gameObjects)
        {
            netServer?.Handle();
            netClient?.Handle();
            foreach (var udp in udps)
                udp.Handle();

            //foreach (GameObject gameObject in gameObjects)
            //{
            //    if (!gameObject.Active)
            //        continue;
            //    NetworkTransform netTransform = gameObject.GetComponent<NetworkTransform>();
            //    if (!netTransform || !netTransform.Active)
            //        continue;

            //    Transform transform = gameObject.GetComponent<Transform>();
            //    Position position = new Position(transform.Position.X, transform.Position.Y);
            //    byte[] data = Serializer.NetSerialize(position);
            //}
        }
    }
}