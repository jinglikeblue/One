using One;
using System;
using System.Collections.Generic;
using System.Text;
using Jing;

namespace OneServer
{
    class CoreModel : BaseModel
    {
        public string[] startupArgs;
        public SettingsConfigVO settings;
        public WebSocketServer server;

        ThreadSyncActions _tsa;

        protected override void OnInit()
        {
            _tsa = new ThreadSyncActions();
        }

        protected override void OnDestroy()
        {
            _tsa.Clear();
        }

        public void RunOnMainThread(Action action)
        {
            _tsa.AddToSyncAction(action);
        }

        public void SyncMainThreadActions()
        {
            _tsa.RunSyncActions();
        }
    }
}
