namespace Destroy.Net
{
    using System;

    public class Client : NetworkClient2
    {
        protected override void OnConnected()
        {
            Console.WriteLine("成功连接上服务器");
        }

        protected override void OnDisConnected()
        {
        }
    }
}