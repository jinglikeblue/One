using One.Net;
using OneDemo.Common;
using System;
using System.Threading;
using Util;

namespace ServerDemos
{
    class StreeTestingServerDemo
    {
        public static void Main(string[] args)
        {
            new StreeTestingServerDemo();
        }

        ThreadSyncActions _tsa = new ThreadSyncActions();
        TcpServer<AsyncSimpleTcpProtocolProcess> _tcpSrver;
        public StreeTestingServerDemo()
        {
            _tcpSrver = new TcpServer<AsyncSimpleTcpProtocolProcess>();            
            _tcpSrver.onClientEnterHandler += OnClientEnter;
            _tcpSrver.onClientExitHandler += OnClientExit;
            _tcpSrver.Start(1875, 4096);

            Console.WriteLine("StreeTestingDemoServer");
            Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId);
            Console.ReadKey();
        }

        private void OnClientEnter(object sender, IRemoteProxy e)
        {
            (e.protocolProcess as AsyncSimpleTcpProtocolProcess).bindClient = e;
            (e.protocolProcess as AsyncSimpleTcpProtocolProcess).onReceiveProtocol += OnReceiveProtocol;
        }

        private void OnClientExit(object sender, IRemoteProxy e)
        {
            (e.protocolProcess as AsyncSimpleTcpProtocolProcess).bindClient = null;
            (e.protocolProcess as AsyncSimpleTcpProtocolProcess).onReceiveProtocol -= OnReceiveProtocol;
        }

        private void OnReceiveProtocol(object sender, byte[] obj)
        {
            var client = (sender as AsyncSimpleTcpProtocolProcess).bindClient;
            //Console.WriteLine("回数据：{0}", Thread.CurrentThread.ManagedThreadId);
            client.Send(obj);
        }
    }
}
