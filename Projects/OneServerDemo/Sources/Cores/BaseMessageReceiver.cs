using One;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    interface IMessageReceiver { }

    /// <summary>
    /// 消息接收器基类
    /// </summary>
    /// <typeparam name="Data"></typeparam>
    abstract class BaseMessageReceiver<Data>:IMessageReceiver
    {
        /// <summary>
        /// 收到协议
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public abstract bool OnReceive(BaseSession session, Data msg);
    }
}
