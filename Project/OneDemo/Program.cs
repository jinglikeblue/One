using One.Net;
using System;
using System.Threading;

namespace One
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

            ClientManager.onClientEnterHandler += OnCLientEnter;
            ClientManager.onClientExitHandler += OnCLientExit;

            Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId); 
            Console.ReadKey();
            //SocketServer ss = new SocketServer(1000, 4096);
            //ss.Init();
            //ss.Start(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1875));
            //MainLoop();
        }

        private void OnCLientEnter(object sender, Client e)
        {
            Console.WriteLine("Thread [{0}]:enter", Thread.CurrentThread.ManagedThreadId);
        }

        private void OnCLientExit(object sender, Client e)
        {
            Console.WriteLine("Thread [{0}]:exit", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
