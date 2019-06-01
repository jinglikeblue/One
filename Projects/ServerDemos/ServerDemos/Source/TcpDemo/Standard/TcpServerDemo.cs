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
            _tcpSrver = new TcpServer();            
            _tcpSrver.onClientEnterHandler += OnClientEnter;
            _tcpSrver.onClientExitHandler += OnClientExit;
            _tcpSrver.Start(1875, 4096);
            
            new Thread(LogicThraed).Start();

            Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId); 
            Console.ReadKey();            
        }

        private void OnClientEnter(IRemoteProxy e)
        {                       
            UserMgr.Ins.Enter(e);                            
        }

        private void OnClientExit(IRemoteProxy e)
        {
            UserMgr.Ins.Exit(e);
        }
        
        /// <summary>
        /// 逻辑线程
        /// </summary>
        private void LogicThraed()
        {
            Console.WriteLine("Thread [{0}]:Logic Start", Thread.CurrentThread.ManagedThreadId);
            int delay = 10;
            while (true)
            {
                _tcpSrver.Refresh();
                UserMgr.Ins.Update();

                Thread.Sleep(delay);
            }
        }
    }
}
