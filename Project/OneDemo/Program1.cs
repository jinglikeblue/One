using One.Net;
using One.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OneDemo
{
    class Program1
    {
        static void Main(string[] args)
        {
            new Program1();
        }

        BaseTcpProtocolProcess _pp;
        TcpSocketClient _client;

        public Program1()
        {
            _client = new TcpSocketClient(new BaseTcpProtocolProcess());
            _client.onConnectSuccess += OnConnectSuccess;
            _client.onConnectFail += OnConnectFail;
            _client.Connect("127.0.0.1", 1875, 4096);

            _pp = _client.protocolProcess as BaseTcpProtocolProcess;
            while (true)
            {
                _pp.ReceiveProtocols(OnReceiveProtocol);
                Thread.Sleep(1000);
            }            
        }

        private void OnReceiveProtocol(BaseTcpProtocolBody obj)
        {
            Console.WriteLine("T{} 收到：{1}",Thread.CurrentThread.ManagedThreadId, obj.value);
            _client.Send(_pp.Pack(obj));
        }

        private void OnConnectSuccess(object sender, TcpSocketClient e)
        {
            Console.WriteLine("连接成功：{0}");
        }

        private void OnConnectFail(object sender, TcpSocketClient e)
        {
            Console.WriteLine("连接失败：{0}");
        }


    }
}
