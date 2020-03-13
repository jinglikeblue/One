using System;
using System.Collections.Generic;
using System.Text;

namespace One.WebSocket
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    public interface IMessageExpress
    {
        object Pack(object messageData);
        void Unpack(object target, object messageData);
    }
}
