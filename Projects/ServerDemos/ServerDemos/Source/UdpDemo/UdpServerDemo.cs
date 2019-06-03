using One;
using System;
using System.Net;
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
            _server.onReceiveData += OnReceiveData;
            _server.Bind(1875, 4096);

            Log.CI(ConsoleColor.DarkGreen, "Logic Thread Start");

            int delay = 10;
            while (true)
            {
                _server.Refresh();
                Thread.Sleep(delay);
            }
        }

        UdpSendChannel sendChannel;

        private void OnReceiveData(UdpServer server, EndPoint ep, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            Log.I("收到数据:{0}", ba.ReadString());

            ba.Reset();
            ba.Write("I Got It");            

            if(null == sendChannel)
            {
                sendChannel = _server.CreateSendChannel(ep);
            }
            
            sendChannel.Send(ba.GetAvailableBytes());

            //server.Send(ba.GetAvailableBytes());
        }
    }
}
