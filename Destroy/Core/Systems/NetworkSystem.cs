namespace Destroy
{
    using System.Collections.Generic;

    public static class NetworkSystem
    {
        private static NetworkServer server;

        private static NetworkClient client;

        private static List<UDPService> udps = new List<UDPService>();

        public static void Init(NetworkServer server, NetworkClient client, List<UDPService> udps)
        {
            NetworkSystem.server = server;
            NetworkSystem.client = client;
            NetworkSystem.udps = udps ?? new List<UDPService>();

            NetworkSystem.server?.Start();
            NetworkSystem.client?.Start();
        }

        internal static void Update(List<GameObject> gameObjects)
        {
            server?.Handle();
            client?.Handle();
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