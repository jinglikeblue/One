using System;
using System.Collections.Generic;
using System.Text;

namespace One
{
    public abstract class WebSocketSession : BaseSession
    {
        protected RootBehavior behavior { get; private set; }

        internal void BindingBehavior(RootBehavior behavior)
        {
            this.behavior = behavior;
            behavior.onClose += OnClose;
            behavior.onError += OnError;
            behavior.onMessage += OnMessage;
            OnOpen();
        }

        public override void Close()
        {
            behavior.SilentlyClose();
            behavior = null;
        }
    }
}
