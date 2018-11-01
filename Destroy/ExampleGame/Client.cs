namespace Destroy.ExampleGame
{
    using System.Threading;

    [CreatGameObject(2, "Client")]
    public class Client : Script
    {
        public override void Start()
        {
            Thread receive = new Thread(Receive) { IsBackground = true };
            Thread send = new Thread(Send) { IsBackground = true };
            receive.Start();
            send.Start();
        }

        public override void Update(float deltaTime)
        {

        }

        void Receive()
        {
            while (true)
            {

            }
        }

        void Send()
        {
            while (true)
            {

            }
        }
    }
}