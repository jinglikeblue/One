using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace One.Net
{
    class ClientManager
    {
        static public void Enter(Socket socketClient)
        {
            new Client(socketClient);
        }
    }
}
