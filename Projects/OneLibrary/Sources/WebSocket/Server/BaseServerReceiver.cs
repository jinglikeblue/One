using Google.Protobuf;

namespace One.WebSocket
{
    /// <summary>
    /// Server端协议接收器基类
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class BaseServerReceiver<TData>
    {
        protected abstract void OnReceive(Session session, TData obj);
    }
}
