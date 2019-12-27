using One;

namespace WebSocketServerDemo
{
    class Session : WebSocketSession
    {
        protected override void OnClose()
        {
            Log.I("连接关闭：{0}", id);
        }

        protected override void OnError()
        {
            Log.I("连接出错：{0}", id);
        }

        protected override void OnMessage(object data)
        {
            if (data is string)
            {
                Log.I("收到消息：{0} data:{1}", id, data);
            }
            else if(data is byte[])
            {
                ByteArray ba = new ByteArray(data as byte[]);
                Log.I("收到消息：{0}", ba.ReadStringBytes(ba.Available));
            }
        }

        protected override void OnOpen()
        {
            Log.I("建立连接：{0}", id);
        }
    }
}
