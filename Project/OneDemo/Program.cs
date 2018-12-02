using One.Net;
using OneDemo.Managers;
using System;
using System.Threading;

namespace OneDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ByteArray.defaultBufferSize = 4096;
            new Program();
        }

        ThreadSyncActions _tsa = new ThreadSyncActions();

        public Program()
        {            
            new SocketServer().Start("0.0.0.0", 1875);
            
            ClientManager.onClientEnterHandler += OnClientEnter;
            ClientManager.onClientExitHandler += OnClientExit;

            new Thread(LogicThraed).Start();

            Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId); 
            Console.ReadKey();            
        }

        private void OnClientEnter(object sender, Client e)
        {
            _tsa.AddToSyncAction(()=> {
                UserMgr.Ins.Enter(e);
                //Console.WriteLine("Thread [{0}]:enter", Thread.CurrentThread.ManagedThreadId);
            });            
        }

        private void OnClientExit(object sender, Client e)
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

            int delay = 1000 / 30;

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
