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
            _client = new TcpClient();
            _client.onConnectSuccess += OnConnectSuccess;
            _client.onReceiveData += OnReceiveProtocol;
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

        private void OnDisconnect(TcpClient e)
        {
            Log.I("连接断开");
        }

        private void OnReceiveProtocol(TcpClient client, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            var last = ba.ReadString();
            long now = DateTime.Now.ToFileTimeUtc();

            Log.CI(ConsoleColor.DarkYellow, "收到消息:{0}", last);            
        }

        private void OnConnectSuccess(TcpClient client)
        {
            Log.I("连接成功");
            Send();
        }

        void Send()
        {
            ByteArray ba = new ByteArray();
            ba.Write(DateTime.Now.ToFileTimeUtc().ToString());            
            _client.Send(ba.GetAvailableBytes());
            ba.SetPos(0);
            Log.CI(ConsoleColor.DarkMagenta, "发送消息:{0}", ba.ReadString());
        }

        private void OnConnectFail(TcpClient client)
        {
            Log.I("连接失败");
        }


    }
}
