namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class UDPRoom
    {
        private UdpClient client;

        public static UDPRoom CreatServer()
        {
            UDPRoom room = new UDPRoom();
            return room;
        }

        public static UDPRoom CreatClient(IPEndPoint iPEndPoint)
        {
            UDPRoom room = new UDPRoom(iPEndPoint);
            return room;
        }

        public UDPRoom()
        {
            //初始化自身IP与端口
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public UDPRoom(IPEndPoint iPEndPoint)
        {
            //初始化自身IP与端口
            client = new UdpClient(iPEndPoint);
        }

        public void BroadCast(byte[] data, int targetPort)
        {
            IPEndPoint target = new IPEndPoint(IPAddress.Broadcast, targetPort);
            client.Send(data, data.Length, target);
        }

        public void Send(byte[] data, IPEndPoint target) => client.Send(data, data.Length, target);

        public byte[] Receive(out IPEndPoint remoteEP)
        {
            remoteEP = null;
            byte[] data = client.Receive(ref remoteEP);
            return data;
        }


        public long Ping(IPEndPoint target, long overTime)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            socket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

            Stopwatch watch = Stopwatch.StartNew();

            //发送一个字节
            byte[] sendData = new byte[1] { 0 };
            IAsyncResult sendAsync = socket.BeginSendTo(sendData, 0, sendData.Length, SocketFlags.None, target, null, null);
            while (!sendAsync.IsCompleted)
            {
                //判断超时
                if (watch.ElapsedMilliseconds >= overTime)
                    return overTime;
                Thread.Sleep(0);
            }
            socket.EndSendTo(sendAsync);

            //接受一个字节
            byte[] receiveData = new byte[1];
            EndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            IAsyncResult receiveAsync = socket.BeginReceiveFrom(receiveData, 0, receiveData.Length, SocketFlags.None, ref iPEndPoint, null, null);
            while (!receiveAsync.IsCompleted)
            {
                //判断超时
                if (watch.ElapsedMilliseconds >= overTime)
                    return overTime;
                Thread.Sleep(0);
            }
            socket.EndReceiveFrom(receiveAsync, ref iPEndPoint);

            //计算时间
            long millis = watch.Elapsed.Milliseconds;
            socket.Close();
            return millis;
        }

        public void ReceivePing()
        {
            byte[] data = Receive(out IPEndPoint remoteEP);
            Send(data, remoteEP);
        }
    }
}