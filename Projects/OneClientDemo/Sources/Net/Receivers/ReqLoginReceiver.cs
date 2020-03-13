using One;
using One.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneClient
{
    class RspLoginReceiver : BaseClientProtobufReceiver<RspLogin>
    {
        protected override void OnReceive(Client client, RspLogin pbObj)
        {
            OneLog.D("登录返回：{0}", pbObj.Id);
        }
    }
}
