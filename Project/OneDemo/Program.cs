using One.Net;
using OneDemo;
using OneDemo.Managers;
using System;
using System.Collections.Generic;
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

        public Program()
        {            
            new SocketServer().Start("0.0.0.0", 1875);
            
            ClientManager.onClientEnterHandler += OnClientEnter;
            ClientManager.onClientExitHandler += OnClientExit;

            new Thread(LogicThraed).Start();

            Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId); 
            Console.ReadKey();
            //SocketServer ss = new SocketServer(1000, 4096);
            //ss.Init();
            //ss.Start(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1875));
            
        }

        private void OnClientEnter(object sender, Client e)
        {
            AddSyncAction(()=> {
                UserMgr.Ins.Enter(e);
                Console.WriteLine("Thread [{0}]:enter", Thread.CurrentThread.ManagedThreadId);
            });
            
            
        }

        private void OnClientExit(object sender, Client e)
        {
            AddSyncAction(() =>
            {
                UserMgr.Ins.Exit(e);
                Console.WriteLine("Thread [{0}]:exit", Thread.CurrentThread.ManagedThreadId);
            });
        }

        List<Action> logicSyncActionList = new List<Action>();

        public void AddSyncAction(Action action)
        {
            lock(logicSyncActionList)
            {
                logicSyncActionList.Add(action);
            }
        }

        
        /// <summary>
        /// 逻辑线程
        /// </summary>
        private void LogicThraed()
        {
            Console.WriteLine("Thread [{0}]:Logic Start", Thread.CurrentThread.ManagedThreadId);

            while (true)
            {
                RunSyncActions();

                UserMgr.Ins.Update();

                Thread.Sleep(1000 / 30);
            }
        }

        /// <summary>
        /// 执行线程同步方法清单
        /// </summary>
        void RunSyncActions()
        {
            List<Action> actionList = new List<Action>();
            actionList.Clear();
            lock (logicSyncActionList)
            {
                actionList.AddRange(logicSyncActionList);
                logicSyncActionList.Clear();
            }

            for (int i = 0; i < actionList.Count; i++)
            {
                actionList[i].Invoke();
            }
        }
    }
}
