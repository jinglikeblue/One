using One;
using One.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OneServer
{
    class LoginRequestReceiver : BaseServerReceiver<LoginRequestVO>
    {
        protected override void OnReceive(Session session, LoginRequestVO obj)
        {
            OneLog.D("[T:{0} S:{1}],登录请求：{2}", Thread.CurrentThread.ManagedThreadId, session.id, obj.nickname);

            LoginResponseVO vo = new LoginResponseVO();
            vo.id = 999;
            session.SendPackage(vo);
        }
    }
}
