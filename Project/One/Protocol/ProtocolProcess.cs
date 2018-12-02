using One.Core;
using One.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace One.Protocol
{
    /// <summary>
    /// 协议处理器  
    /// 协议包结构为 | ushort：协议数据长度 | 协议数据 |
    /// </summary>
    class ProtocolProcess : IProtocolProcess
    {
        public Queue<ProtocolBody> pbList = new Queue<ProtocolBody>();

        /// <summary>
        /// 解包协议数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>使用的数据长度</returns>
        public int Unpack(byte[] buffer, int available)
        {
            ByteArray ba = new ByteArray(buffer, available);
            int used = 0;
            Unpack(ba, ref used);
            return used;
        }

        void Unpack(ByteArray ba, ref int used)
        {
            if(ba.ReadEnableSize < ByteArray.USHORT_SIZE)
            {
                return;
            }

            //获取协议数据长度
            ushort protocolSize = ba.ReadUShort();
            if(ba.ReadEnableSize < protocolSize)
            {
                //数据存在半包问题
                return;
            }

            byte[] protocolData = ba.ReadBytes(protocolSize);
            ProtocolBody pb = new ProtocolBody();
            pb.Unserialize(protocolData);
            //协议加入收到的协议队列
            pbList.Enqueue(pb);

            //记录使用协议长度
            used += ByteArray.USHORT_SIZE + protocolData.Length;

            //迭代下一个粘连包
            Unpack(ba, ref used);
        }

        /// <summary>
        /// 将协议数据打包为可以直接发送的字节数组
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public byte[] Pack(IProtocolBody pb)
        {
            byte[] pbData = pb.Serialize();            
            ByteArray ba = new ByteArray(pbData.Length + ByteArray.USHORT_SIZE);
            ushort dataSize = (ushort)pbData.Length;
            //写入协议数据长度
            ba.Write(dataSize);
            //写入协议数据
            ba.Write(pbData);
            return ba.Bytes;
        }
    }
}
