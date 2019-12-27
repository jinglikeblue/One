using System;
using System.Collections.Generic;
using System.Text;

namespace One
{
    public abstract class WebSocketSession : BaseSession
    {
        RootBehavior _behavior;

        internal void BindingBehavior(RootBehavior behavior)
        {
            _behavior = behavior;
            behavior.onClose += OnClose;
            behavior.onError += OnError;
            behavior.onMessage += OnMessage;
            OnOpen();
        }

        public override void Close()
        {
            _behavior.SilentlyClose();
            _behavior = null;
        }

        public override void Send(object data)
        {
            if (data is byte[])
            {
                _behavior.SendData((byte[]) data);
            }
            else
            {
                _behavior.SendData((string)data);
            }            
        }
    }
}
