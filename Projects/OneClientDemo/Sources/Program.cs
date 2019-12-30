﻿using System;
using One;
using Share;

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
            new InitMsgInfoTableCommand().Excute();
            Global.Ins.net.ws.Connect("127.0.0.1", 1875);
            Global.Ins.net.ws.onOpen += OnOpen;
        }

        private static void OnOpen()
        {
            var vo = new LoginRequestVO();
            vo.deviceId = "a";
            vo.account = "tester";
            vo.pwd = "hello";
            Global.Ins.net.ws.Send(vo);
        }
    }
}