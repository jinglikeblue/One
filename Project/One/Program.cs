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
            Console.WriteLine("Press any key to terminate the server process....");
            Console.ReadKey();
            //SocketServer ss = new SocketServer(1000, 4096);
            //ss.Init();
            //ss.Start(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1875));
            //MainLoop();
        }

        void MainLoop()
        {
            while (true)
            {                
                Thread.Sleep(1000);
            }
        }
    }
}
