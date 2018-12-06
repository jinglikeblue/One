using System;
using System.Threading;

namespace ClientDemos
{
    internal class StreeTestingDemoClient
    {
        public static string host;
        public static int port;
        public static int count;
        public static int sendSize;

        static void Main(string[] args)
        {
            host = args[0];
            port = int.Parse(args[1]);
            count = int.Parse(args[2]);
            sendSize = int.Parse(args[3]);

            for (int i = 0; i < count; i++)
            {
                var t = new StreeTestingClient(i);
                new Thread(t.Start).Start();
            }

            Console.ReadKey();
        }
    }
}
