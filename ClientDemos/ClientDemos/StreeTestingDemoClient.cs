using System;
using System.Collections.Generic;
using System.Text;

namespace OneDemo
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
