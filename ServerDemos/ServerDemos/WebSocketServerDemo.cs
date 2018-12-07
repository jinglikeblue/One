﻿using One.Data;
using One.Net;
using One.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerDemos
{
    class WebSocketServerDemo
    {
        WebSocketServer<WebSocketTextProtocolProcess> _server;

        public static void Main(string[] args)
        {           
            ByteArray.defaultBufferSize = 4096;
            new WebSocketServerDemo();
        }

        public WebSocketServerDemo()
        {
            _server = new WebSocketServer<WebSocketTextProtocolProcess>();
            _server.Start("0.0.0.0", 1875, 80000);

            //new Thread(LogicThraed).Start();

            //Console.WriteLine("Thread [{0}]:Press any key to terminate the server process....", Thread.CurrentThread.ManagedThreadId);
            Console.ReadKey();
        }
    }
}
