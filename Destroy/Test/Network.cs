namespace Destroy.Test
{
    /// <summary>
    /// TODO
    /// </summary>
    public class Server : Script
    {
        public override void Start()
        {
            NetworkServer server = new NetworkServer(8848);
            

            NetworkSystem.Init(server, null, null);

        }

    }
}