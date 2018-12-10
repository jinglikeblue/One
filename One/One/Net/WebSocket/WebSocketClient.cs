using System;
using One.Protocol;

namespace One.Net
{
    public class WebSocketClient : TcpSocketClient, IRemoteProxy
    {
        public WebSocketClient(IProtocolProcess protocolProcess) : base(protocolProcess)
        {
            
        }
    }
}
