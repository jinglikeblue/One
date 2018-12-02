using One.Core;
using One.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace One.Protocol
{
    /// <summary>
    /// 协议处理器
    /// </summary>
    class ProtocolProcess : IProtocolProcess
    {
        /// <summary>
        /// 解包协议数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>使用的数据长度</returns>
        public int Unpack(byte[] buffer)
        {
            new ByteArray(buffer);
            return 0;
        }

        /// <summary>
        /// 将协议数据打包为可以直接发送的字节数组
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public byte[] Pack(IProtocolBody pb)
        {
            return pb.Serialize();
        }
    }
}
