using System;
using System.Collections.Generic;
using System.Text;

namespace Share
{
    /// <summary>
    /// 消息包
    /// </summary>
    public class MsgPackageVO
    {
        /// <summary>
        /// 消息的ID
        /// </summary>
        public int id;

        /// <summary>
        /// 请求ID，如果该值>0，则标记了这次请求的识别码
        /// </summary>
        public long requestId = 0;

        /// <summary>
        /// 消息的内容
        /// </summary>
        public string content;
    }
}
