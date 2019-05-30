using One;
using System;

namespace ServerDemo
{
    /// <summary>
    /// 最简单的协议处理，用以测试高并发
    /// </summary>
    public class AsyncSimpleTcpProtocolProcess : IProtocolProcess
    {
        public IRemoteProxy bindClient;

        public event EventHandler<byte[]> onReceiveProtocol;

        public void SetSender(IRemoteProxy sender)
        {
            
        }

        public int Unpack(byte[] buf, int available)
        {
            ByteArray ba = new ByteArray(buf, available);
            onReceiveProtocol?.Invoke(this, ba.GetAvailableBytes());
            return available;
        }
    }
}
