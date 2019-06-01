using One;
using System;
using System.Threading;

namespace ServerDemo
{
    class TcpServerDemo
    {
        public static void Main(string[] args)
        {
            ByteArray.defaultBufferSize = 4096;
            new TcpServerDemo();
        }

        TcpServer _tcpSrver;
        public TcpServerDemo()
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
            _tcpSrver = new TcpServer();
            _tcpSrver.onClientEnter += OnClientEnter;
            _tcpSrver.onClientExit += OnClientExit;
            _tcpSrver.Start(1875, 4096);

            Log.CI(ConsoleColor.DarkGreen, "Logic Thread Start");
            int delay = 10;
            while (true)
            {
                _tcpSrver.Refresh();
                UserMgr.Ins.Update();

                Thread.Sleep(delay);
            }
        }

        private void OnClientEnter(IChannel e)
        {
            Log.I("用户进入");
            UserMgr.Ins.Enter(e);
        }

        private void OnClientExit(IChannel e)
        {
            Log.I("用户退出");
            UserMgr.Ins.Exit(e);
        }
    }
}
