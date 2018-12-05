using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy
{
    /// <summary>
    /// TODO
    /// </summary>
    [CreatGameObject]
    public class Server : Script
    {
        public override void Start()
        {
            NetworkServer server = new NetworkServer(8848);
            

            NetworkSystem.Init(server, null, null);

        }

    }
}