using One;
using One.WebSocket;
using OneServer;
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

        ProtobufExpress pe = new ProtobufExpress();

        JsonExpress je = new JsonExpress();

        public Program()
        {
            pe.AutoRegister(typeof(OneMsgId), typeof(BaseClientReceiver<>));

            je.AutoRegister(GetType().Assembly, typeof(BaseClientReceiver<>));

            //new InitMsgInfoTableCommand().Excute();
            client = new Client();
            client.messageExpress = je;
            client.onOpen += OnOpen;
            client.Connect("127.0.0.1", 1875);
            //Global.Ins.net.ws.Connect("127.0.0.1", 1875);            

            Console.ReadKey();
        }

        private void OnOpen(Client client)
        {
            LoginRequestVO msg = new LoginRequestVO();
            msg.account = "jing";
            msg.nickname = "帅到想毁容";

            //ReqLogin msg = new ReqLogin();
            //msg.Nickname = "jing";
            client.SendPackage(msg);
        }
    }
}
