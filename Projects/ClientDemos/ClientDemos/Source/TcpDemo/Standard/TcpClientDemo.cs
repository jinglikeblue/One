using One;
using System;
using System.Threading;

namespace ClientDemo
{
    class TcpClientDemo
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                new TcpClientDemo();
            }
        }

        BaseTcpProtocolProcess _pp;
        TcpClient _client;        

        public TcpClientDemo()
        {
            _client = new TcpClient(new BaseTcpProtocolProcess());
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
                Thread.Sleep(1000);
            }            
        }

        private void OnDisconnect(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接断开：{0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void OnReceiveProtocol(BaseTcpProtocolBody obj)
        {
            long last = long.Parse(obj.value);
            long now = DateTime.Now.ToFileTimeUtc();

            Log.CI(ConsoleColor.DarkYellow, "[{0}] 收到消息:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), obj.value);

            //Console.WriteLine("T{0} 消息延迟：{1}",Thread.CurrentThread.ManagedThreadId, (now - last) / 10000);
        }

        private void OnConnectSuccess(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接成功：{0}", Thread.CurrentThread.ManagedThreadId);
            Send();
        }

        void Send()
        {
            BaseTcpProtocolBody obj = new BaseTcpProtocolBody();
            obj.value = DateTime.Now.ToFileTimeUtc().ToString();
            _client.Send(_pp.Pack(obj));
            Log.CI(ConsoleColor.DarkMagenta, "[{0}] 发送消息:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), obj.value);
        }

        private void OnConnectFail(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接失败：{0}", Thread.CurrentThread.ManagedThreadId);
        }


    }
}
