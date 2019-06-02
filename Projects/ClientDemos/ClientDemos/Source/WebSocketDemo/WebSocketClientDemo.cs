using One;
using System;
using System.Text;
using System.Threading;

namespace ClientDemo
{
    class WebSocketClientDemo
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1; i++)
            {
                new WebSocketClientDemo();
            }

            Console.ReadKey();
        }

        WebSocketClient _client;

        public WebSocketClientDemo()
        {
            _client = new WebSocketClient();            
            _client.onConnectSuccess += OnConnectSuccess;
            _client.onDisconnect += OnDisconnect;
            _client.onConnectFail += OnConnectFail;
            _client.onReceiveData += OnReceiveProtocol;
            _client.Connect("127.0.0.1", 1875, 4096);

            while (true)
            {
                _client.Refresh();
                if (_client.IsConnected)
                {
                    //client.Refresh();
                    Send();
                }
                Thread.Sleep(1000);
            }
        }

        void Send()
        {
            ByteArray ba = new ByteArray();
            ba.WriteStringBytes(DateTime.Now.ToFileTimeUtc().ToString());
            _client.Send(ba.GetAvailableBytes());
            ba.SetPos(0);
            Log.CI(ConsoleColor.DarkMagenta, "发送消息:{0}", ba.ReadStringBytes(ba.Available));
        }

        private void OnDisconnect(WebSocketClient e)
        {
            Log.I("连接断开");
        }

        private void OnReceiveProtocol(WebSocketClient client, byte[] obj)
        {
            var ba = new ByteArray(obj);
            Log.I("服务器返回消息：{0}", ba.ReadStringBytes(ba.Available));
        }

        private void OnConnectSuccess(WebSocketClient e)
        {
            Log.I("连接成功");
            //_client.SendData("hello");
            //Send();
        }

        private void OnConnectFail(WebSocketClient e)
        {
            Log.I("连接失败");
        }
    }
}
