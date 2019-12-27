using System;
using System.Collections.Generic;
using System.Text;

namespace One
{
    public abstract class WebSocketSession : BaseSession
    {
        internal void BindingBehavior(RootBehavior behavior)
        {
            behavior.onClose += OnClose;
            behavior.onError += OnError;
            
        }

        public override void Close()
        {
            
        }

        public override void Send(object data)
        {
            
        }
    }
}
