namespace Destroy.Net
{
    using System.Net.Sockets;

    public class Server : NetworkServer
    {
        public Server(int port) : base(port)
        {
        }

        protected override void OnAccept(TcpClient tcpClient)
        {

        }
    }
}