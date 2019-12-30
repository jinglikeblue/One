using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace One
{
    public class RootBehavior : WebSocketBehavior
    {
        public event Action onClose;
        public event Action onError;
        public event Action<object> onMessage;   
        
        BaseSession _session;

        protected override void OnClose(CloseEventArgs e)
        {
            onClose?.Invoke();
        }

        protected override void OnError(ErrorEventArgs e)
        {
            onError?.Invoke();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                onMessage?.Invoke(e.Data);
            }
            else
            {
                onMessage?.Invoke(e.RawData);
            }              
        }

        public void SendData(string data)
        {
            Send(data);
        }

        public void SendData(byte[] data)
        {
            Send(data);            
        }

        protected override void OnOpen()
        {
            var sessions = WebSocketServer.current.sessions;
            var session = sessions.Create() as WebSocketSession;
            session.BindingBehavior(this);
        }

        public void SilentlyClose()
        {
            onClose = null;
            onError = null;
            onMessage = null;
            var sessions = WebSocketServer.current.sessions;
            sessions.Destroy(_session.id);
            Close();
        }
    }
}
