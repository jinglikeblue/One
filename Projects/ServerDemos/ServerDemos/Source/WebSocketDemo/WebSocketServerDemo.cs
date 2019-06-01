using One;
using System;

namespace ServerDemo
{
    class WebSocketServerDemo
    {
        WebSocketServer _server;

        public static void Main(string[] args)
        {           
            ByteArray.defaultBufferSize = 4096;
            new WebSocketServerDemo();
        }

        public WebSocketServerDemo()
        {
            _server = new WebSocketServer();
            _server.onClientEnter += OnClientEnter;
            _server.onClientExit += OnClientExit;
            _server.Start(1875, 80000);
            

            //new Thread(LogicThraed).Start();

            //Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId);
            Console.ReadKey();
        }

        private void OnClientExit(IRemoteProxy e)
        {
            
        }

        private void OnClientEnter(IRemoteProxy e)
        {
            (e.protocolProcess as WebSocketProtocolProcess).onReceiveProtocolEvent += OnReceiveData;
        }

        private void OnReceiveData(IRemoteProxy sender, byte[] data)
        {
            //收到的数据原路返回(Test)
            (sender as WebSocketRemoteProxy).SendData(data);
        }
    }
}
