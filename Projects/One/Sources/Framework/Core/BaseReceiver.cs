using System;
using System.Collections.Generic;
using System.Text;

namespace One
{
    /// <summary>
    /// 协议接收器
    /// </summary>
    public abstract class BaseReceiver
    {
        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="data"></param>
        public abstract void OnMessage(BaseSession session, object data);

        /// <summary>
        /// 在主线程执行方法
        /// </summary>
        /// <param name="action"></param>
        public void RunOnMainThread(Action action)
        {

        }
    }
}
