using Newtonsoft.Json;
using One;
using Share;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    class LoginRequestReceiver : BaseMessageReceiver<LoginRequestVO>
    {
        public override bool OnReceive(BaseSession session, LoginRequestVO msg)
        {
            Log.I("收到协议:" + JsonConvert.SerializeObject(msg));            
            return true;
        }
    }
}
