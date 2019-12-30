using One;
using System;

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

            _server = new WebSocketServer(1875);
            //_server.RegisterSeesionType();
            //_server.AddWebSocketService<Echo>("/");
            //_server.AddReceiverType(typeof(DataReceiver));
            _server.Start();
            
            One.Log.I(ConsoleColor.DarkGreen, "Press any key to terminate the server process....");
            Console.ReadKey();
        }
    }
}
