using System;
using System.Threading;
using WebSocketSharp.Server;

namespace WebSocketServerDemo
{
    class Program
    {
        static void Main(string[] args)
        {            
            new Program();
        }

        WebSocketServer _server;

        public Program()
        {

            _server = new WebSocketServer("ws://0.0.0.0:1875");
            //_server.WaitTime = new TimeSpan(100);
            _server.AddWebSocketService<Echo>("/");
            _server.Start();

            One.Log.CI(ConsoleColor.DarkGreen, "Press any key to terminate the server process....");
            Console.ReadKey();
        }
    }
}
