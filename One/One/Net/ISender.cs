namespace One.Net
{
    /// <summary>
    /// 可以发送数据的对象
    /// </summary>
    public interface ISender
    {
        void Send(byte[] data);
    }
}
