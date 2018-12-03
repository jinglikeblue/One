namespace One.Core
{
    /// <summary>
    /// 协议处理器实现接口
    /// </summary>
    interface IProtocolProcess
    {
        /// <summary>
        /// 将协议体打包为通信接口发送的字节数组
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        byte[] Pack(IProtocolBody body);

        /// <summary>
        /// 将通信接口收到的字节数据解包
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        int Unpack(byte[] buf, int available);
    }
}
