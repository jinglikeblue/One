using Jing;
using Newtonsoft.Json;
using One;
using One.WebSocket;
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
            }
            catch (Exception e)
            {
                OneLog.E(e.Message);
            }
        }

        void Startup()
        {
            
            //读取服务器配置
            var settingsPath = "../../../Configs/settings.json";
            var content = File.ReadAllText(settingsPath);                  
            _core.settings = JsonConvert.DeserializeObject<SettingsConfigVO>(content);

            InitMessageExpress();
            //启动通信服务
            _core.server = new Server(_core.settings.port);
            _core.server.onNewSession += OnNewSession;
            _core.server.Start();

            OneLog.I(ConsoleColor.DarkYellow, "WebSocket Server Start! Lisening... {0}:{1}", _core.server.host, _core.server.port);

            //日志控制
            OneLog.isConsoleOutput = _core.settings.logConsoleEnable;
            if (_core.settings.logOutputEnable)
            {
                OneLog.UseLogFile(_core.settings.logOutputDir, _core.settings.logKeepDays);
            }                      
            
            RegisterMainLogicLoopCommand();

            //redis初始化
            RedisMgr.Ins.Connect();

            //执行测试代码 
            //new Tests.TestMain();

            //全局异常捕获
            AppDomain.CurrentDomain.UnhandledException += OnException;

            //如果服务器没有收到关闭信号，则一直执行
            while (false == _core.isExit)
            {                
                //执行同步的线程方法
                _core.SyncMainThreadActions();
                //执行主线程循环逻辑代码
                _core.RunMainLogicLoop();
                Thread.Sleep(_core.settings.mainLogicLoopIntervalMS);
            }
        }

        ProtobufExpress pe = new ProtobufExpress();

        void InitMessageExpress()
        {
            pe.AutoRegister(typeof(OneMsgId), typeof(BaseServerProtobufReceiver<>));
        }

        private void OnNewSession(Session obj)
        {
            obj.messageExpress = pe;
        }

        private void OnException(object sender, UnhandledExceptionEventArgs e)
        {
            OneLog.E("捕获到异常:");
            OneLog.E(e.ToString());
        }

        /// <summary>
        /// 注册主逻辑循环调用指令
        /// </summary>
        void RegisterMainLogicLoopCommand()
        {            
            _core.RegisterMainLogicLoop(new CheckCloseCommand());
        }        
    }
}
