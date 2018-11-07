using Boxhead.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Player
{
    public int Id;
    public Socket Socket;
    public ConcurrentQueue<PlayerInput> Inputs;

    public Player(int id, Socket socket)
    {
        Id = id;
        Socket = socket;
        Inputs = new ConcurrentQueue<PlayerInput>();
    }
}