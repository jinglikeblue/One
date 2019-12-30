using One;
using System;
using System.Collections.Generic;
using System.Text;
using Jing;

namespace OneServer
{
    /// <summary>
    /// 核心模型
    /// </summary>
    class CoreModel : BaseModel
    {
        public string[] startupArgs;

        public SettingsConfigVO settings;

        public WebSocketServer server;

        ThreadSyncActions _tsa;

        List<IMainLoopLogic> _mainLoopLogicList;

        public bool isExit = false;

        public CoreModel()
        {
            _tsa = new ThreadSyncActions();
            _mainLoopLogicList = new List<IMainLoopLogic>();
        }

        protected override void Reset()
        {
            _tsa.Clear();
            _mainLoopLogicList.Clear();
        }

        /// <summary>
        /// 将方法放在主线程执行
        /// </summary>
        /// <param name="action"></param>
        public void RunOnMainThread(Action action)
        {
            _tsa.AddToSyncAction(action);
        }

        public void SyncMainThreadActions()
        {
            _tsa.RunSyncActions();
        }

        /// <summary>
        /// 注册一个主逻辑每帧执行的逻辑代码
        /// </summary>
        /// <param name="logic"></param>
        public void RegisterMainLogicLoop(IMainLoopLogic logic)
        {
            _mainLoopLogicList.Add(logic);
        }

        public void RunMainLogicLoop()
        {
            foreach(var logic in _mainLoopLogicList)
            {
                logic.Excute();
            }
        }
    }
}
