using Jing;
using One;
using System;
using System.IO;
using System.Threading;

namespace OneServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.Ins.core.startupArgs = args;
            
            new Program();
        }

        CoreModel _core;

        public Program()
        {
            try
            {
                _core = Global.Ins.core;
                new Thread(Startup).Start();
                //while (false == _core.isExit)
                //{
                //    string input = Console.ReadLine();
                //    if(input == "exit")
                //    {
                //        _core.isExit = true;
                //    }
                //    Log.I("指令:{0}", input);
                //}
            }
            catch (Exception e)
            {
                Log.E(e.Message);
            }
        }

        void Startup()
        {
            var settingsPath = "../../../Configs/settings.json";
            var content = File.ReadAllText(settingsPath);
            _core.settings = LitJson.JsonMapper.ToObject<SettingsConfigVO>(content);
            _core.server = new WebSocketServer(_core.settings.port);
            //注册连接Session类型
            _core.server.RegisterSeesionType(typeof(Session));
            _core.server.Start();

            Log.I(ConsoleColor.DarkGreen, "WebSocket Server Start! Lisening... {0}:{1}", _core.server.host, _core.server.port);            

            _core.RegisterMainLogicLoop(new CheckCloseCommand());

            while (false == _core.isExit)
            {                
                //执行同步的线程方法
                _core.SyncMainThreadActions();
                //执行主线程循环逻辑代码
                _core.RunMainLogicLoop();
                Thread.Sleep(_core.settings.mainLogicLoopIntervalMS);
            }
        }
    }
}
