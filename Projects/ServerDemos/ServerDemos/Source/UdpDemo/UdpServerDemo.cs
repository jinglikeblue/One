using One;
using System;
using System.Threading;

namespace ServerDemo
{
    public class UdpServerDemo
    {
        public static void Main(string[] args)
        {
            ByteArray.defaultBufferSize = 4096;
            new UdpServerDemo();
        }

        private UdpServer _server;

        public UdpServerDemo()
        {


            new Thread(LogicThraed).Start();

            Console.ReadKey();
        }

        /// <summary>
        /// 逻辑线程
        /// </summary>
        private void LogicThraed()
        {
            _server = new UdpServer();
            _server.onReceiveData += OnReceiveDataEvent;
            _server.Start(1875, 4096);

            Log.CI(ConsoleColor.DarkGreen, "Logic Thread Start");

            int delay = 10;
            while (true)
            {
                _server.Refresh();
                Thread.Sleep(delay);
            }
        }

        private void OnReceiveDataEvent(UdpChannel sender, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            Log.I("收到数据:{0}", ba.ReadString());

            ba.Reset();
            ba.Write("I Got It");
            sender.Send(ba.GetAvailableBytes());
        }
    }
}
