namespace Destroy.Net
{
    using System;
    using System.Collections.Generic;
    using Destroy;

    public static class NetworkSystem
    {
        private static Dictionary<int, Instantiate> prefabs;

        public static GameClient Client;

        private static GameServer server;

        private static bool useNet;

        private static float clientInterval;

        private static float serverInterval;

        private static bool choose;

        private static float serverTimer;

        private static float clientTimer;

        internal static void Init(bool useNet, int clientSyncRate, int serverBroadcastRate)
        {
            NetworkSystem.useNet = useNet;
            clientInterval = (float)1 / clientSyncRate;
            serverInterval = (float)1 / serverBroadcastRate;
        }

        public static void Register(Dictionary<int, Instantiate> prefabs) => NetworkSystem.prefabs = prefabs;

        internal static void Update(List<GameObject> gameObjects)
        {
            if (!useNet)
                return;
            if (prefabs == null)
                throw new Exception("Please Call NetworkSystem.Register if you useNet");

            if (!choose)
            {
                Console.WriteLine("1.Client, 2.Server");
                int mode = int.Parse(Console.ReadLine());
                switch (mode)
                {
                    case 1:
                        Client = new GameClient(NetworkUtils.LocalIPv4Str, 8848, prefabs);
                        Client.Start();
                        break;
                    case 2:
                        server = new GameServer(8848, prefabs);
                        server.Start();
                        break;
                    default:
                        throw new Exception();
                }
                choose = true;
                Console.Clear();
            }

            if (server != null)
            {
                serverTimer += Time.DeltaTime;
                server.Update();
                if (serverTimer >= serverInterval)
                {
                    serverTimer = 0;
                    server.Broadcast();
                }
            }
            if (Client != null)
            {
                clientTimer += Time.DeltaTime;
                Client.Update();
                if (clientTimer >= clientInterval)
                {
                    clientTimer = 0;
                    Client.Move();
                }
            }
        }
    }
}