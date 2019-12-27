using One;
using System;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace WebSocketClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1; i++)
            {
                new Program();
            }

            Console.ReadKey();
        }

        WebSocket _client;


        public Program()
        {
            _client = new WebSocket("ws://127.0.0.1:1875");
            _client.OnOpen += OnConnectSuccess;
            _client.OnMessage += OnMessage;
            _client.OnClose += OnClose;
            _client.OnError += OnError;
            _client.Connect();

            while (true)
            {
                if (_client.IsAlive)
                {
                    Send();
                }
                Thread.Sleep(1000);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Log.I("连接失败");
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Log.I("连接断开");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var ba = new ByteArray(e.RawData);
            Log.I("服务器返回消息：{0}", ba.ReadStringBytes(ba.Available));
        }

        private void OnConnectSuccess(object sender, EventArgs e)
        {
            Log.I("连接成功");
        }

        void Send()
        {
            ByteArray ba = new ByteArray();
            ba.WriteStringBytes(DateTime.Now.ToFileTimeUtc().ToString());
            _client.Send(ba.GetAvailableBytes());
            ba.SetPos(0);

            Log.I(ConsoleColor.DarkMagenta, "发送消息:{0}", ba.ReadStringBytes(ba.Available));
        }
    }
}
