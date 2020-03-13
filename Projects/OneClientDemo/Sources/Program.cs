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
            //这部分是前后端通用的，因为收发都要用
            pe.RegisterMsg(OneMsgId.ReqLogin, typeof(ReqLogin));
            pe.RegisterMsg(OneMsgId.RspLogin, typeof(RspLogin));            

            //这部分是处理服务器发过来的协议
            pe.RegisterReceiver(OneMsgId.RspLogin, typeof(RspLoginReceiver));

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
