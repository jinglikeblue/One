using System;
using System.Collections.Generic;
using System.Text;
using One.Data;
using One.Net;

namespace One.Protocol
{
    public class BaseUdpProtocolProcess : IProtocolProcess
    {
        List<byte[]> _pbList = new List<byte[]>();

        /// <summary>
        /// 用传入的委托方法来接收协议处理器收集到的协议（线程安全）
        /// </summary>
        /// <param name="onReceiveProtocol"></param>
        public void ReceiveProtocols(Action<byte[]> onReceiveProtocol)
        {
            List<byte[]> pbList = new List<byte[]>();

            lock (_pbList)
            {
                pbList.AddRange(_pbList);
                _pbList.Clear();
            }

            for (int i = 0; i < pbList.Count; i++)
            {
                onReceiveProtocol(pbList[i]);
            }
        }

        public void SetSender(IRemoteProxy sender)
        {
            
        }

        public int Unpack(byte[] buf, int available)
        {
            ByteArray ba = new ByteArray(buf, available);
            lock (_pbList)
            {
                //协议加入收到的协议队列
                _pbList.Add(ba.ReadBytes(available));
            }
            return available;
        }
    }
}
