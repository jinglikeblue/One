using One.Data;
using One.Net;
using One.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClientDemos
{
    class UdpSocketClientDemo
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1; i++)
            {
                new UdpSocketClientDemo();
            }

            Console.ReadKey();
        }

        UdpClient _client;
        BaseUdpProtocolProcess _pp;

        public UdpSocketClientDemo()
        {
            _pp = new BaseUdpProtocolProcess();
            var client = new UdpClient(_pp);            
            _client = client;
            _client.Bind("127.0.0.1", 1875, 1874, 4096);            
            for (int i = 0; i < 1000; i++)
            {
                _client.Send(Encoding.UTF8.GetBytes("Hello" + i));
            }

            _pp.onReceiveEvent += OnReceiveEvent;
            //_client.Connect("121.40.165.18", 8800, 4096);

            //_pp = _client.protocolProcess as WebSocketProtocolProcess;
            //while (true)
            //{
            //    if (_client.IsConnected)
            //    {
            //        _pp.ReceiveProtocols(OnReceiveProtocol);
            //        //Send();
            //    }
            //    Thread.Sleep(1000);
            //}
        }

        private void OnReceiveEvent(object sender, ByteArray e)
        {
            var s = Encoding.UTF8.GetString(e.GetAvailableBytes());
            Console.WriteLine("服务器返回消息：{1}", Thread.CurrentThread.ManagedThreadId, s);
        }

        private void OnReceiveProtocol(ByteArray ba)
        {
            var s = Encoding.UTF8.GetString(ba.GetAvailableBytes());
            //long last = long.Parse(obj.value);
            //long now = DateTime.Now.ToFileTimeUtc();
            Console.WriteLine("服务器返回消息：{1}", Thread.CurrentThread.ManagedThreadId, s);
        }

        private void OnDisconnect(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接断开：{0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void OnConnectSuccess(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接成功：{0}", Thread.CurrentThread.ManagedThreadId);
            //_client.SendData("hello");
            //Send();
        }

        private void OnConnectFail(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接失败：{0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
