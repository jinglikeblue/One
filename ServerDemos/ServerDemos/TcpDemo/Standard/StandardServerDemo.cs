using One.Data;
using One.Net;
using One.Protocol;
using OneDemo.Managers;
using System;
using System.Threading;
using Util;

namespace ServerDemos
{
    class StandardServerDemo
    {
        public static void Main(string[] args)
        {
            ByteArray.defaultBufferSize = 4096;
            new StandardServerDemo();
        }

        ThreadSyncActions _tsa = new ThreadSyncActions();
        TcpServer<BaseTcpProtocolProcess> _tcpSrver;
        public StandardServerDemo()
        {
            _tcpSrver = new TcpServer<BaseTcpProtocolProcess>();            
            _tcpSrver.onClientEnterHandler += OnClientEnter;
            _tcpSrver.onClientExitHandler += OnClientExit;
            _tcpSrver.Start(1875, 4096);
            
            new Thread(LogicThraed).Start();

            Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId); 
            Console.ReadKey();            
        }

        private void OnClientEnter(object sender, IRemoteProxy e)
        {
            //Console.WriteLine("Thread [{0}]:enter cout:{1}", Thread.CurrentThread.ManagedThreadId, _tcpSrver.ClientCount);
            _tsa.AddToSyncAction(()=> {
                UserMgr.Ins.Enter(e);                
            });            
        }

        private void OnClientExit(object sender, IRemoteProxy e)
        {
            _tsa.AddToSyncAction(() =>
            {
                UserMgr.Ins.Exit(e);
                //Console.WriteLine("Thread [{0}]:exit", Thread.CurrentThread.ManagedThreadId);
            });
        }
        
        /// <summary>
        /// 逻辑线程
        /// </summary>
        private void LogicThraed()
        {
            Console.WriteLine("Thread [{0}]:Logic Start", Thread.CurrentThread.ManagedThreadId);

            //long last = DateTime.UtcNow.ToFileTimeUtc() / 10000;
            //long ms = 0;
            //long maxDetal = 0;

            //int delay = 1000 / 60;
            int delay = 10;
            while (true)
            {
                //long now = DateTime.UtcNow.ToFileTimeUtc() / 10000;
                //long detal = now - last;
                //last = now;
                //if (detal > maxDetal)
                //{
                //    maxDetal = detal;
                //}
                //ms += detal;                
                //if (ms >= 1000)
                //{
                //    Console.WriteLine("Time [{0}]",  maxDetal);
                //    ms = 0;
                //    maxDetal = 0;
                //}

                _tsa.RunSyncActions();

                UserMgr.Ins.Update();

                Thread.Sleep(delay);
            }
        }
    }
}
