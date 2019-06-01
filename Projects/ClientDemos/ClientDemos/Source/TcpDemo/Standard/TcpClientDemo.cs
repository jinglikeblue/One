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
        
        TcpClient _client;        

        public TcpClientDemo()
        {
            _client = new TcpClient(new TcpProtocolProcess());
            _client.onConnectSuccess += OnConnectSuccess;
            _client.onDisconnect += OnDisconnect;
            _client.onConnectFail += OnConnectFail;
            _client.Connect("127.0.0.1", 1875, 4096);

            
            while (true)
            {
                _client.Refresh();
                if (_client.IsConnected)
                {                    
                    Send();
                }
                Thread.Sleep(1000);
            }            
        }

        private void OnDisconnect(IRemoteProxy e)
        {
            Console.WriteLine("连接断开：{0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void OnReceiveProtocol(byte[] obj)
        {
            ByteArray ba = new ByteArray(obj);
            var last = ba.ReadString();
            long now = DateTime.Now.ToFileTimeUtc();

            Log.CI(ConsoleColor.DarkYellow, "[{0}] 收到消息:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), last);

            //Console.WriteLine("T{0} 消息延迟：{1}",Thread.CurrentThread.ManagedThreadId, (now - last) / 10000);
        }

        private void OnConnectSuccess(IRemoteProxy e)
        {
            Console.WriteLine("连接成功：{0}", Thread.CurrentThread.ManagedThreadId);
            Send();
        }

        void Send()
        {
            ByteArray ba = new ByteArray();
            ba.Write(DateTime.Now.ToFileTimeUtc().ToString());            
            _client.Send(ba.GetAvailableBytes());
            ba.SetPos(0);
            Log.CI(ConsoleColor.DarkMagenta, "[{0}] 发送消息:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ba.ReadString());
        }

        private void OnConnectFail(IRemoteProxy e)
        {
            Console.WriteLine("连接失败：{0}", Thread.CurrentThread.ManagedThreadId);
        }


    }
}
