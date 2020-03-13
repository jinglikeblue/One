using One;
using One.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    class ReqLoginReceiver : BaseServerProtobufReceiver<ReqLogin>
    {
        protected override void OnReceive(Session client, ReqLogin pbObj)
        {
            OneLog.D("登录请求：{0}", pbObj.Nickname);
            RspLogin rsp = new RspLogin();
            rsp.Id = 1;
            client.SendPackage(rsp);
        }
    }
}
