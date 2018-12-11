using One.Data;
using One.Net;
using One.Protocol;
using System;

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
            _server.onReceiveDataEvent += OnReceiveDataEvent;
            _server.Start(1875, 4096);

            Console.ReadKey();
        }

        private void OnReceiveDataEvent(object sender, UdpRemoteProxy remoteProxy)
        {
            var pp = (remoteProxy.protocolProcess as BaseUdpProtocolProcess);
            pp.ReceiveProtocols((ByteArray ba) =>
            {
                remoteProxy.Send(ba.GetAvailableBytes());
            });
        }
    }
}
