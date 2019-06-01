using System;

namespace One
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
        /// <returns>返回使用了的字节数。重要，如果传错会影响数据读取</returns>
        int Unpack(byte[] buf, int available, Action<byte[]> onReceiveData);

        /// <summary>
        /// 将协议数据打包为可以直接发送的字节数组
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        byte[] Pack(byte[] protocolData);
    }
}
