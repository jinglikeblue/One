using System.Threading;

namespace One
{
    /// <summary>
    /// 协议的数据结构体
    /// </summary>
    public struct BaseTcpProtocolBody
    {      
        public string value;

        public byte[] Serialize()
        {            
            ByteArray ba = new ByteArray();
            ba.Write(value);
            return ba.GetAvailableBytes();
        }

        public void Unserialize(byte[] protocolData)
        {
            ByteArray ba = new ByteArray(protocolData);
            value = ba.ReadString();
            //Log.I("{0}: 收到的内容：{1}", Thread.CurrentThread.ManagedThreadId, value);            
        }
    }

    
}
