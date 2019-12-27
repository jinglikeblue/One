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

        public Program()
        {
            try
            {
                Startup();                
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
            Global.Ins.core.settings = LitJson.JsonMapper.ToObject<SettingsConfigVO>(content);
            Global.Ins.core.server = new WebSocketServer(Global.Ins.core.settings.port);

            Global.Ins.core.server.Start();

            while (true)
            {
                Global.Ins.core.SyncMainThreadActions();
                Thread.Sleep(100);
            }
        }
    }
}
