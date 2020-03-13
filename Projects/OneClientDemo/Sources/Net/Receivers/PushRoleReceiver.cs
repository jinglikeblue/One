using One;
using One.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneServer
{
    class PushRoleReceiver : BaseClientReceiver<PushRole>
    {
        protected override void OnReceive(Client client, PushRole pbObj)
        {
            OneLog.D("新的用户：{0}", pbObj.Id);
        }
    }
}
