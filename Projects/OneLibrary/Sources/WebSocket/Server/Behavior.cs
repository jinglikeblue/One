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
            onOpen?.Invoke();                
        }

        protected override void OnClose(CloseEventArgs e)
        {         
            onClose?.Invoke(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {         
            onError?.Invoke(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            onMessage?.Invoke(e);            
        }

        public new void Send(string str)
        {
            base.Send(str);
        }

        public new void Send(byte[] bytes)
        {               
            base.Send(bytes);
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
            try
            {
                base.Close();
            }
            catch(Exception e)
            {
                OneLog.W(e.Message);
            }
        }
    }
}
