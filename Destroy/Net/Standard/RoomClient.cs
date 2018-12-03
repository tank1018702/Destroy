namespace Destroy
{
    using System.Threading;
    using System.Net;

    public class RoomClient
    {
        private UDPRoom client;

        private Thread send;
        private Thread receive;

        public RoomClient(IPEndPoint iPEndPoint)
        {
            client = UDPRoom.CreatClient(iPEndPoint);
            send = null;
            receive = null;
        }

        public void Open()
        {
            send = new Thread(__Send) { IsBackground = true };
            send.Start();

            receive = new Thread(__Receive) { IsBackground = true };
            receive.Start();
        }

        private void __Receive()
        {
            while (true)
            {

            }
        }

        private void __Send()
        {
            while (true)
            {

            }
        }
    }
}