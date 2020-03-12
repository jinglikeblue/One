using One.WebSocket;
using System;

namespace OneClient
{
    class Program
    {       
        static void Main(string[] args)
        {
            new Program();
        }

        Client client;

        public Program()
        {
            //new InitMsgInfoTableCommand().Excute();
            client = new Client();
            client.onOpen += OnOpen;
            client.Connect("127.0.0.1", 1875);
            //Global.Ins.net.ws.Connect("127.0.0.1", 1875);            

            Console.ReadKey();
        }

        private void OnOpen()
        {
            client.Send(MessageUtility.TransformData("hello world"));
        }
    }
}
