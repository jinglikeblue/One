using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace One
{
    class RootBehavior : WebSocketBehavior
    {
        public event Action onClose;
        public event Action onError;
        public event Action<MessageEventArgs> onMessage;

        SessionCenter sessions;

        protected override void OnClose(CloseEventArgs e)
        {                       
            
        }

        protected override void OnError(ErrorEventArgs e)
        {
            
        }

        protected override void OnMessage(MessageEventArgs e)
        {
                
        }

        protected override void OnOpen()
        {
            var session = sessions.Create() as WebSocketSession;
            session.BindingBehavior(this);
        }
    }
}
