using Google.Protobuf;

namespace One.WebSocket
{
    /// <summary>
    /// 客户端协议接收器基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class BaseClientReceiver<TData>
    {
        protected abstract void OnReceive(Client client, TData obj);
    }
}
