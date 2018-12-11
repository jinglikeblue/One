using One.Net;
using OneDemo.Common;
using System;
using System.Text;
using System.Threading;

namespace ClientDemos
{
    /// <summary>
    /// 服务器压力测试，客户端类。将收到的服务器协议立刻发送回去
    /// </summary>
    class StreeTestingClient
    {
        static byte[] bytes;

        AsyncSimpleTcpProtocolProcess _pp;
        TcpClient _client;
        int _id;

        public StreeTestingClient(int id)
        {
            _id = id;
            bytes = new byte[StreeTestingDemoClient.sendSize];
        }

        public void Start()
        {
            _pp = new AsyncSimpleTcpProtocolProcess();
            _pp.onReceiveProtocol += OnReceiveProtocol;

            _client = new TcpClient(_pp);
            _client.onConnectSuccess += OnConnectSuccess;
            _client.onDisconnect += OnDisconnect;
            //_client.Connect("127.0.0.1", 1875, 4096);
            //_client.Connect("192.168.31.229", 1875, 4096);
            _client.Connect(StreeTestingDemoClient.host, StreeTestingDemoClient.port, (ushort)StreeTestingDemoClient.sendSize);

            while (true)
            {
                if (_client.IsConnected)
                {
                    Send();
                    //Console.WriteLine("发送：{0}", Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1000);
                }
            }
        }

        private void OnReceiveProtocol(object sender, byte[] obj)
        {
            //Console.WriteLine("回数据：{0}", Thread.CurrentThread.ManagedThreadId);
            //_client.Send(obj);
        }

        private void OnDisconnect(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接断开：{0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void OnConnectSuccess(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接成功：{0}", Thread.CurrentThread.ManagedThreadId);
                       
    
        }

        void Send()
        {
            //UTF8Encoding uft8 = new UTF8Encoding();
            //_client.Send(uft8.GetBytes("test"));
            _client.Send(bytes);
        }
    }
}
