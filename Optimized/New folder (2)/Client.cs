using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Text;

public class Client : MonoBehaviour
{
    public GameObject Other;

    public string Address;
    public int Port;

    private TcpClient client;
    private NetworkStream stream;
    //降低写入时的GC
    private MemoryStream memory;
    private BinaryWriter writer;

    void Start()
    {
        pos = Other.transform.position; //位置初始化

        client = null;
        stream = null;
        memory = new MemoryStream();
        writer = new BinaryWriter(memory, Encoding.UTF8);

        StartCoroutine(Connect());
    }

    void NetUpdate(byte netFrame, byte otherMove)
    {
        Debug.Log($"收到服务发来的第:{netFrame}, 距离上一帧{Time.fixedDeltaTime}");
        //Input
        StartCoroutine(Send());
        //Output
        if (otherMove == 1)
        {
            //一秒调10次, 一次走0.05米, 一秒走0.5米, 2个一起控制一秒走1米。
            pos += new Vector3(2.5f, 0, 0) * Time.fixedDeltaTime;
        }
    }

    private void FixedUpdate()
    {
        Vector3 vector = Vector3.Lerp(Other.transform.position, pos, 0.2f);
        Other.transform.position = vector;
    }

    Vector3 pos;

    IEnumerator Connect()
    {
        client = new TcpClient();
        IAsyncResult async = client.BeginConnect(IPAddress.Parse(Address), Port, null, null);
        while (!async.IsCompleted)
            yield return null;

        client.EndConnect(async);
        stream = client.GetStream();
        //接受服务器调用NetUpdate
        StartCoroutine(Receive());
    }

    IEnumerator Receive()
    {
        while (true)
        {
            byte[] data = new byte[2];
            IAsyncResult async = stream.BeginRead(data, 0, data.Length, null, null);
            while (!async.IsCompleted)
                yield return null;

            stream.EndRead(async);

            using (MemoryStream memory = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(memory))
                {
                    byte netFrame = reader.ReadByte();      //网络帧数         1字节
                    byte otherInput = reader.ReadByte();    //另一个玩家的输入  1字节
                    //调用NetUpdate
                    NetUpdate(netFrame, otherInput);
                }
            }
        }
    }

    IEnumerator Send()
    {
        memory.Position = 0;
        writer.Write((byte)1); //1字节
        memory.Flush();

        IAsyncResult async = stream.BeginWrite(memory.GetBuffer(), 0, (int)memory.Position, null, null);
        while (!async.IsCompleted)
            yield return null;

        stream.EndWrite(async);
    }
}