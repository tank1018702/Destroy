namespace Destroy
{
    using System;
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

        static bool choose = false;

        internal static void Update(List<GameObject> gameObjects)
        {
            //if (!choose)
            //{
            //    choose = true;
            //    Print.DrawLine("1.client, 2.server", ConsoleColor.White);
            //    int input = int.Parse(Console.ReadLine());
            //    switch (input)
            //    {
            //        case 1:
            //            {
            //                Print.DrawLine("1.client, 2.server", ConsoleColor.White);
                            
            //            }
            //            break;
            //        case 2:
            //            {

            //            }
            //            break;
            //        default:
            //            {
            //                throw new Exception();
            //            }
            //    }
            //    Console.Clear();
            //}


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