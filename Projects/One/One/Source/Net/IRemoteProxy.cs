namespace One
{
    /// <summary>
    /// 远端连接的代理，通过该代理与远端对象通信
    /// </summary>
    public interface IRemoteProxy
    {
        /// <summary>
        /// 发送数据给远端
        /// </summary>
        /// <param name="data">二进制数据内容</param>
        void Send(byte[] data);

        /// <summary>
        /// 关闭远端代理
        /// </summary>
        void Close();
    }
}
