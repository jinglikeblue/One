using Google.Protobuf;

namespace One.WebSocket
{
    /// <summary>
    /// Server端协议接收器基类
    /// </summary>
    /// <typeparam name="TProtobuf"></typeparam>
    public abstract class BaseServerProtobufReceiver<TProtobuf> where TProtobuf: Google.Protobuf.IMessage,new()
    {
        protected abstract void OnReceive(Session session, TProtobuf pbObj);
    }
}
