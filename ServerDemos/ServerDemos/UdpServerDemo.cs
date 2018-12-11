using One.Data;
using One.Net;
using One.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerDemos
{
    public class UdpServerDemo
    {
        public static void Main(string[] args)
        {
            ByteArray.defaultBufferSize = 4096;
            new UdpServerDemo();
        }

        private UdpServer<BaseUdpProtocolProcess> _server;

        public UdpServerDemo()
        {
            _server = new UdpServer<BaseUdpProtocolProcess>();
            _server.onClientEnterHandler += OnClientEnter;
            _server.onClientExitHandler += OnClientExit;
            _server.Start(1875, 4096);

            Console.ReadKey();
        }

        private void OnClientExit(object sender, IRemoteProxy e)
        {
            
        }

        private void OnClientEnter(object sender, IRemoteProxy e)
        {
            var data = Encoding.UTF8.GetBytes("hello");
            e.Send(data);
        }
    }
}
