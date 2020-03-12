using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace One.WebSocket
{
    public class Behavior : WebSocketBehavior
    {
        Action onOpen;
        Action<CloseEventArgs> onClose;
        Action<ErrorEventArgs> onError;
        Action<MessageEventArgs> onMessage;

        public void BindingCallback(Action onOpen, Action<CloseEventArgs> onClose, Action<ErrorEventArgs> onError, Action<MessageEventArgs> onMessage)
        {
            this.onOpen = onOpen;
            this.onClose = onClose;
            this.onError = onError;
            this.onMessage = onMessage;            
        }

        protected override void OnOpen()
        {
            Console.WriteLine("连接打开");
            onOpen?.Invoke();                
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("连接关闭");
            onClose?.Invoke(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("连接出错");
            onError?.Invoke(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            onMessage?.Invoke(e);
        }

        public new void Send(byte[] data)
        {               
            base.Send(data);
        }

        /// <summary>
        /// 关闭会话，并且不会触发任何事件
        /// </summary>
        public void CloseSliently()
        {
            onOpen = null;
            onClose = null;
            onError = null;
            onMessage = null;
            base.CloseAsync();
        }
    }
}
