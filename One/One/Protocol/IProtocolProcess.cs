using One.Net;

namespace One.Protocol
{
    /// <summary>
    /// 协议处理器实现接口
    /// </summary>
    public interface IProtocolProcess
    {
        /// <summary>
        /// Socket连接创建协议处理器时，会将关联的发送器传递进来。可根据需要保留引用。
        /// </summary>
        /// <param name="sender"></param>
        void SetSender(ISender sender);

        /// <summary>
        /// 将通信接口收到的字节数据解包（多线程方法）
        /// </summary>
        /// <param name="buf">缓冲区数组</param>
        /// <param name="available">可以读取的字节数</param>
        /// <returns>返回使用了的字节数。重要，如果传错会影响数据读取</returns>
        int Unpack(byte[] buf, int available);
    }
}
