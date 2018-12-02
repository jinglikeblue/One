namespace One.Core
{
    /// <summary>
    /// 协议数据体
    /// </summary>
    interface IProtocolBody
    {
        /// <summary>
        /// 反序列化协议数据
        /// </summary>
        /// <param name="protocolData"></param>
        void Unserialize(byte[] protocolData);

        /// <summary>
        /// 序列化协议数据
        /// </summary>
        /// <returns></returns>
        byte[] Serialize();
    }
}
