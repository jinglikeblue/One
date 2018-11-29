using One.Net;
using System;
using System.Net;
using System.Threading;

namespace One
{
    class Program
    {
        static void Main(string[] args)
        {           
            new Program();
        }

        public Program()
        {
            SocketServer ss = new SocketServer(1000, 4096);
            ss.Init();
            ss.Start(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1875));
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
