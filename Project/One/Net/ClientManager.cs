using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace One.Net
{
    public class ClientManager
    {
        public static event EventHandler<Client> onClientEnterHandler;
        public static event EventHandler<Client> onClientExitHandler;
        //public Action<Client> onClientEnter;
        //public Action<Client> onClientExit;

        static public void Enter(Socket socketClient)
        {            
            Client client = new Client(socketClient, 4096);
            onClientEnterHandler?.Invoke(null, client);
        }

        static public void Exit(Client client)
        {
            onClientExitHandler?.Invoke(null, client);
        }
    }
}
