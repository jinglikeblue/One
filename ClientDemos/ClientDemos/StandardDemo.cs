using One.Net;
using One.Protocol;
using System;
using System.Threading;

namespace OneDemo
{
    class StandardDemo
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                new StandardDemo();
            }
        }

        BaseTcpProtocolProcess _pp;
        TcpSocketClient _client;        

        public StandardDemo()
        {
            _client = new TcpSocketClient(new BaseTcpProtocolProcess());
            _client.onConnectSuccess += OnConnectSuccess;
            _client.onDisconnect += OnDisconnect;
            _client.onConnectFail += OnConnectFail;
            _client.Connect("127.0.0.1", 1875, 4096);

            _pp = _client.protocolProcess as BaseTcpProtocolProcess;
            while (true)
            {
                if (_client.IsConnected)
                {
                    _pp.ReceiveProtocols(OnReceiveProtocol);
                    Send();
                }
                Thread.Sleep(1);
            }            
        }

        private void OnDisconnect(object sender, TcpSocketClient e)
        {
            Console.WriteLine("连接断开：{0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void OnReceiveProtocol(BaseTcpProtocolBody obj)
        {
            long last = long.Parse(obj.value);
            long now = DateTime.Now.ToFileTimeUtc();            
            Console.WriteLine("T{0} 消息延迟：{1}",Thread.CurrentThread.ManagedThreadId, (now - last) / 10000);
        }

        private void OnConnectSuccess(object sender, TcpSocketClient e)
        {
            Console.WriteLine("连接成功：{0}", Thread.CurrentThread.ManagedThreadId);
            Send();
        }

        void Send()
        {
            BaseTcpProtocolBody obj = new BaseTcpProtocolBody();
            obj.value = DateTime.Now.ToFileTimeUtc().ToString();
            _client.Send(_pp.Pack(obj));
        }

        private void OnConnectFail(object sender, TcpSocketClient e)
        {
            Console.WriteLine("连接失败：{0}", Thread.CurrentThread.ManagedThreadId);
        }


    }
}
