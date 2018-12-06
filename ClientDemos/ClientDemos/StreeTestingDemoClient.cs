using System;
using System.Threading;

namespace ClientDemos
{
    class StreeTestingDemoClient
    {
        static void Main(string[] args)
        {
            int count = 5000;
            for (int i = 0; i < count; i++)
            {
                var t = new StreeTestingClient(i);
                new Thread(t.Start).Start();
            }

            Console.ReadKey();
        }
    }
}
