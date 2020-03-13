using One;
using One.WebSocket;
using System.Threading;

namespace OneServer
{
    class ReqMsgReceiver : BaseServerProtobufReceiver<ReqMsg>
    {
        protected override void OnReceive(Session session, ReqMsg pbObj)
        {
            OneLog.D("[T:{0} S:{1}],发送消息：{2}", Thread.CurrentThread.ManagedThreadId, session.id, pbObj.Content);

            var role = Global.Ins.roles.FindRole(session);
            if(null == role)
            {
                session.Close();
                return;
            }


            PushMsg pushMsg = new PushMsg();
            pushMsg.Id = role.id;
            pushMsg.Content = pbObj.Content;
            session.sessionManager.PushPackage(pushMsg);
        }
    }
}
