using One;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketServerDemo
{
    class Echo : WebSocketBehavior
    {
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            One.Log.I("连接关闭：{0}", this.ID);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            One.Log.I("连接出错：{0}", this.ID);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            if (e.IsText)
            {
                One.Log.I("收到消息：{0}", e.Data);
                Send(e.Data);
            }
            else if (e.IsBinary)
            {
                ByteArray ba = new ByteArray(e.RawData);
                One.Log.I("收到消息：{0}", ba.ReadStringBytes(ba.Available));
                Send(e.RawData);
            }
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            One.Log.I("建立连接：{0}", this.ID);
        }
    }
}
