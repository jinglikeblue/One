using System;
using System.Threading;

namespace ClientDemo
{
    internal class StreeTestingClientDemo
    {
        public static string host;
        public static int port;
        public static int count;
        public static int sendSize;

        static void Main(string[] args)
        {
            if (args.Length == 4)
            {
                host = args[0];
                port = int.Parse(args[1]);
                count = int.Parse(args[2]);
                sendSize = int.Parse(args[3]);
            }
            else
            {
                Console.WriteLine("启动参数不正确，将使用默认参数");                
                host = "127.0.0.1";
                port = 1875;
                count = 100;
                sendSize = 4096;
            }            

            for (int i = 0; i < count; i++)
            {
                var t = new StreeTestingClientThread(i);
                new Thread(t.Start).Start();
            }

            Console.ReadKey();
        }
    }
}
