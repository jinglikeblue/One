using System;
using System.Collections.Generic;
using System.Text;

namespace Share
{
    /// <summary>
    /// 协议信息数据
    /// </summary>
    class MsgInfoVO
    {
        /// <summary>
        /// 协议ID
        /// </summary>
        public int id;

        /// <summary>
        /// 协议名称
        /// </summary>
        public string name;

        /// <summary>
        /// 数据类型
        /// </summary>
        public Type dataType;

        /// <summary>
        /// 接收器类型
        /// </summary>
        public Type receiverType;
    }
}
