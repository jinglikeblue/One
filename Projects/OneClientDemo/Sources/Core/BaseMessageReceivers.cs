using System;

namespace OneClient
{
    /// <summary>
    /// 消息接收器基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    abstract class BaseMessageReceiver<TData>
    {
        public static Type GetDataType()
        {
            return typeof(TData);
        }

        /// <summary>
        /// 收到协议
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract void OnReceive(long requestId, TData data);
    }
}
