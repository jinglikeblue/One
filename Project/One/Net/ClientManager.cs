using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace One.Net
{
    public class ClientManager
    {
        static int _clientCount = 0;

        /// <summary>
        /// 客户端总数
        /// </summary>
        public int clientCount
        {
            get
            {
                return _clientCount;
            }
        }

        /// <summary>
        /// 新的客户端进入的事件（非线程安全）
        /// </summary>
        public static event EventHandler<Client> onClientEnterHandler;

        /// <summary>
        /// 客户端退出的事件（非线程安全）
        /// </summary>
        public static event EventHandler<Client> onClientExitHandler;

        static internal void Enter(Socket socketClient)
        {
            Interlocked.Increment(ref _clientCount);
            Client client = new Client(socketClient, 4096);
            onClientEnterHandler?.Invoke(null, client);
        }

        static internal void Exit(Client client)
        {
            Interlocked.Decrement(ref _clientCount);
            onClientExitHandler?.Invoke(null, client);

        }
    }
}
