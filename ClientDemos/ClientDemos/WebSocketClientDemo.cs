﻿using One.Net;
using One.Protocol;
using System;
using System.Text;
using System.Threading;

namespace ClientDemos
{
    class WebSocketClientDemo
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1; i++)
            {
                new WebSocketClientDemo();
            }

            Console.ReadKey();
        }

        WebSocketClient _client;
        WebSocketProtocolProcess _pp;

        public WebSocketClientDemo()
        {
            var client = new WebSocketClient();
            _pp = client.protocolProcess as WebSocketProtocolProcess;
            _client = client;            
            _client.onConnectSuccess += OnConnectSuccess;
            _client.onDisconnect += OnDisconnect;
            _client.onConnectFail += OnConnectFail;
            //_client.Connect("127.0.0.1", 1875, 4096);
            _client.Connect("121.40.165.18", 8800, 4096);

            //_pp = _client.protocolProcess as WebSocketProtocolProcess;
            while (true)
            {
                if (_client.IsConnected)
                {
                    _pp.ReceiveProtocols(OnReceiveProtocol);
                    //Send();
                }
                Thread.Sleep(1000);
            }
        }

        private void OnDisconnect(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接断开：{0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void OnReceiveProtocol(byte[] obj)
        {
            var s = Encoding.UTF8.GetString(obj);
            //long last = long.Parse(obj.value);
            //long now = DateTime.Now.ToFileTimeUtc();
            Console.WriteLine("T{0} 消息：{1}", Thread.CurrentThread.ManagedThreadId, s);
        }

        private void OnConnectSuccess(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接成功：{0}", Thread.CurrentThread.ManagedThreadId);
            _client.Send("hello");
            //Send();
        }        

        private void OnConnectFail(object sender, IRemoteProxy e)
        {
            Console.WriteLine("连接失败：{0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
