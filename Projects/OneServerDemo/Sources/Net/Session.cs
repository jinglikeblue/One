using Jing;
using One;
using System;

namespace OneServer
{
    class Session : WebSocketSession
    {
        protected override void OnClose()
        {
            Log.I(ConsoleColor.DarkCyan, "连接关闭：{0}", id);
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
            else if (data is byte[])
            {
                ByteArray ba = new ByteArray(data as byte[]);
                Log.I("收到消息：{0}", ba.ReadStringBytes(ba.Available));
            }

            Send(data);
        }

        protected override void OnOpen()
        {
            Log.I(ConsoleColor.DarkMagenta, "建立连接：{0}", id);
        }
    }
}
