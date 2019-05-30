using System;
using System.Collections.Generic;

namespace One
{
    public class BaseUdpProtocolProcess : IProtocolProcess
    {
        List<ByteArray> _pbList = new List<ByteArray>();

        public event EventHandler<ByteArray> onReceiveEvent;

        /// <summary>
        /// 用传入的委托方法来接收协议处理器收集到的协议（线程安全）
        /// </summary>
        /// <param name="onReceiveProtocol"></param>
        public void ReceiveProtocols(Action<ByteArray> onReceiveProtocol)
        {
            List<ByteArray> pbList = new List<ByteArray>();

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
            if (null != onReceiveEvent)
            {
                onReceiveEvent.Invoke(this, ba);
            }
            else
            {
                lock (_pbList)
                {
                    //协议加入收到的协议队列
                    _pbList.Add(ba);
                }
            }
            return available;
        }
    }
}
