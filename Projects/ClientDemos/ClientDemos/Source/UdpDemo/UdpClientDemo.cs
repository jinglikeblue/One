using One;
using System;
using System.Text;
using System.Threading;

namespace ClientDemo
{
    class UdpClientDemo
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1; i++)
            {
                new UdpClientDemo();
            }

            Console.ReadKey();
        }

        UdpClient _client;

        public UdpClientDemo()
        {            
            var client = new UdpClient();            
            _client = client;
            _client.onReceiveData += OnReceiveEvent;
            _client.Bind("127.0.0.1", 1875, 1874, 4096);

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

        void Send()
        {
            ByteArray ba = new ByteArray();
            ba.Write(DateTime.Now.ToFileTimeUtc().ToString());
            _client.Send(ba.GetAvailableBytes());
            ba.SetPos(0);
            Log.CI(ConsoleColor.DarkMagenta, "发送消息:{0}", ba.ReadString());
        }

        private void OnReceiveEvent(UdpClient sender, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            Log.I("服务器返回消息：{0}", ba.ReadString());            
        }
    }
}
