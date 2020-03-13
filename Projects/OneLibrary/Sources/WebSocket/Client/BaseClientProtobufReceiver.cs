using Google.Protobuf;

namespace One.WebSocket
{
    /// <summary>
    /// 客户端Protobuf协议接收器基类
    /// </summary>
    /// <typeparam name="TProtobuf"></typeparam>
    public abstract class BaseClientProtobufReceiver<TProtobuf> where TProtobuf : Google.Protobuf.IMessage, new()
    {
        protected abstract void OnReceive(Client client, TProtobuf pbObj);
    }
}
