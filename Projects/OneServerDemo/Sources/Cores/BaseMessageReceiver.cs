using One;
using System;

namespace OneServer
{
    /// <summary>
    /// 消息接收器基类
    /// </summary>
    /// <typeparam name="Data"></typeparam>
    abstract class BaseMessageReceiver<Data>
    {
        public static Type GetDataType()
        {
            return typeof(Data);
        }

        /// <summary>
        /// 收到协议
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract void OnReceive(BaseSession session, long requestId, Data data);
    }
}
