namespace One.Protocol
{
    /// <summary>
    /// 协议处理器实现接口
    /// </summary>
    public interface IProtocolProcess
    {
        /// <summary>
        /// 将通信接口收到的字节数据解包（多线程方法）
        /// </summary>
        /// <param name="buf">缓冲区数组</param>
        /// <param name="available">可以读取的字节数</param>
        /// <returns></returns>
        int Unpack(byte[] buf, int available);
    }
}
