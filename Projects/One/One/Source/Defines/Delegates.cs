namespace One
{
    /// <summary>
    /// 接受数据的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public delegate void ReceiveDataEvent(IRemoteProxy sender, byte[] data);
}
