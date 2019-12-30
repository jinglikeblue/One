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
        protected override void OnReceive(BaseSession session, long requestId, LoginRequestVO data)
        {            
            Log.I("收到协议:" + JsonConvert.SerializeObject(data));
            var vo = new LoginResponseVO();
            vo.nickname = "fuck";
            session.Send(vo);
        }
    }
}
