using System;
using System.Threading;

namespace One
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            new Program();
        }

        public Program()
        {
            MainLoop();
        }

        void MainLoop()
        {
            while (true)
            {
                Console.WriteLine(DateTime.UtcNow.Second.ToString());
                Thread.Sleep(1000);
            }
        }
    }
}
