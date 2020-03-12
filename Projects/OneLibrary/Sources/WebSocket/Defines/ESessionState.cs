using System;
using System.Collections.Generic;
using System.Text;

namespace One.WebSocket
{
    /// <summary>
    /// 会话连接状态
    /// </summary>
    public enum ESessionState
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        IDLE,

        /// <summary>
        /// 开启状态
        /// </summary>
        OPEN,

        /// <summary>
        /// 关闭状态
        /// </summary>
        CLOSE,

        /// <summary>
        /// 出错状态
        /// </summary>
        ERROR,
    }
}
