namespace Destroy.Net
{
    public class Client : NetworkClient
    {
        public Client(string serverIp, int serverPort) : base(serverIp, serverPort)
        {

        }

        protected override void OnConnect()
        {

        }
    }
}