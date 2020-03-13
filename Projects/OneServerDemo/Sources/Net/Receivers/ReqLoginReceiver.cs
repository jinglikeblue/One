using One;
using One.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OneServer
{
    class ReqLoginReceiver : BaseServerProtobufReceiver<ReqLogin>
    {
        protected override void OnReceive(Session session, ReqLogin pbObj)
        {
            OneLog.D("[T:{0} S:{1}],登录请求：{2}", Thread.CurrentThread.ManagedThreadId, session.id, pbObj.Nickname);
            RspLogin rsp = new RspLogin();
            var account = pbObj.Account;
            rsp.Id = 1;
            session.SendPackage(rsp);
            
            //TODO 收到登录请求，判断account是否有匹配的数据，如果有则关联，没有则创建
        }
    }
}
