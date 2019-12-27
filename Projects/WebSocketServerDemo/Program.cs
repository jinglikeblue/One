using One;
using System;
using System.Threading;

namespace WebSocketServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ByteArray.defaultBufferSize = 4096;
            new Program();
        }

        WebSocketServer _server;

        public Program()
        {

            new Thread(LogicThraed).Start();

            Log.CI(ConsoleColor.DarkGreen, "Press any key to terminate the server process....");
            Console.ReadKey();
        }

        /// <summary>
        /// 逻辑线程
        /// </summary>
        private void LogicThraed()
        {
            _server = new WebSocketServer();
            _server.onClientEnter += OnClientEnter;
            _server.onClientExit += OnClientExit;
            _server.Start(1875, 80000);

            Log.CI(ConsoleColor.DarkGreen, "Logic Thread Start");
            int delay = 10;
            while (true)
            {
                _server.Refresh();

                Thread.Sleep(delay);
            }
        }

        private void OnClientExit(IChannel e)
        {
            e.onReceiveData -= OnReceiveData;
        }

        private void OnClientEnter(IChannel e)
        {
            e.onReceiveData += OnReceiveData;
        }

        private void OnReceiveData(IChannel remoteProxy, byte[] data)
        {
            ByteArray ba = new ByteArray(data);
            Log.I("收到消息：{0}", ba.ReadStringBytes(ba.Available));
            remoteProxy.Send(data);
        }
    }
}
