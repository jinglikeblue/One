using System;
using System.Threading;

namespace ClientDemos
{
    class StreeTestingDemoClient
    {
        static void Main(string[] args)
        {
            int count = 1000;
            for (int i = 0; i < count; i++)
            {
                new StreeTestingClient(i);
            }

            Console.ReadKey();
        }
    }
}
