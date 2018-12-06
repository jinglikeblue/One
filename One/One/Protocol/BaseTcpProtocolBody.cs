using One.Data;

namespace One.Protocol
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
            //Console.WriteLine("{0}: 收到的内容：{1}", Thread.CurrentThread.ManagedThreadId, value);
        }
    }

    
}
