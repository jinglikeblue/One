using Jing;
using One;
using System;
using System.Threading;

namespace TcpServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ByteArray.defaultBufferSize = 4096;
            new Program();
        }

        TcpServer _tcpSrver;

        public Program()
        {
            new Thread(LogicThraed).Start();

            Log.I(ConsoleColor.DarkGreen, "Press any key to terminate the server process....");
            Console.ReadKey();
        }

        /// <summary>
        /// 逻辑线程
        /// </summary>
        private void LogicThraed()
        {
            _tcpSrver = new TcpServer();
            _tcpSrver.onClientEnter += OnClientEnter;
            _tcpSrver.onClientExit += OnClientExit;
            _tcpSrver.Start(1875, 4096);

            Log.I(ConsoleColor.DarkGreen, "Logic Thread Start");
            int delay = 10;
            while (true)
            {
                _tcpSrver.Refresh();
                Thread.Sleep(delay);
            }
        }

        private void OnClientEnter(IChannel channel)
        {
            //Log.I("用户进入");
            channel.onReceiveData += OnReceiveData;
        }

        private void OnClientExit(IChannel channel)
        {
            //Log.I("用户退出");
            channel.onReceiveData -= OnReceiveData;
        }

        private void OnReceiveData(IChannel sender, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            //Log.I("收到消息:{0}", ba.ReadString());

            ba.Reset();
            ba.Write("Server Got It!");
            sender.Send(ba.GetAvailableBytes());
        }
    }
}
