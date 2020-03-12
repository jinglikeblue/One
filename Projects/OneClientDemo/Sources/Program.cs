using System;

namespace OneClient
{
    class Program
    {       
        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            //new InitMsgInfoTableCommand().Excute();
            //Global.Ins.net.ws.onOpen += OnOpen;
            //Global.Ins.net.ws.Connect("127.0.0.1", 1875);            

            Console.ReadKey();
        }

        private static void OnOpen()
        {
            //var vo = new LoginRequestVO();
            //vo.deviceId = "a";
            //vo.account = "tester";
            //vo.pwd = "hello";
            //Global.Ins.net.Send(vo);
        }
    }
}
