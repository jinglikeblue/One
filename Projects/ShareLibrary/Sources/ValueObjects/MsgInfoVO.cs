using System;
using System.Reflection;

namespace Share
{
    /// <summary>
    /// 协议信息数据
    /// </summary>
    public class MsgInfoVO
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

        /// <summary>
        /// 接收器的接收方法
        /// </summary>
        public MethodInfo receiveMethodInfo;
    }
}
