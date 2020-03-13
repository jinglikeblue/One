using One;
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

        ProtobufExpress pe = new ProtobufExpress();

        public Program()
        {
            pe.AutoRegister(typeof(OneMsgId), typeof(BaseClientReceiver<>));

            //new InitMsgInfoTableCommand().Excute();
            client = new Client();
            client.messageExpress = pe;
            client.onOpen += OnOpen;
            client.Connect("127.0.0.1", 1875);
            //Global.Ins.net.ws.Connect("127.0.0.1", 1875);            

            Console.ReadKey();
        }

        private void OnOpen(Client client)
        {
            ReqLogin msg = new ReqLogin();
            msg.Nickname = "jing";
            client.SendPackage(msg);
        }
    }
}
