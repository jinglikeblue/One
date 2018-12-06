using One.Data;
using One.Net;
using One.Protocol;
using System;

namespace OneDemo.Common
{
    /// <summary>
    /// 最简单的协议处理，用以测试高并发
    /// </summary>
    public class AsyncSimpleTcpProtocolProcess : IProtocolProcess
    {
        public TcpClient bindClient;

        public event EventHandler<byte[]> onReceiveProtocol;

        public int Unpack(byte[] buf, int available)
        {
            ByteArray ba = new ByteArray(buf, available);
            onReceiveProtocol?.Invoke(this, ba.GetAvailableBytes());
            return available;
        }
    }
}
