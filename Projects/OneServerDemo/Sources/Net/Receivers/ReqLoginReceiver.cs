using One;
using One.WebSocket;
using System.Threading;

namespace OneServer
{
    class ReqLoginReceiver : BaseServerProtobufReceiver<ReqLogin>
    {
        protected override void OnReceive(Session session, ReqLogin pbObj)
        {
            OneLog.D("[T:{0} S:{1}],登录请求：{2}", Thread.CurrentThread.ManagedThreadId, session.id, pbObj.Nickname);            

            var role = Global.Ins.roles.GetRole(pbObj.Account, pbObj.Nickname);
            Global.Ins.room.AddRole(role);

            RspLogin rsp = new RspLogin();
            rsp.Id = role.id;
            //rsp.RoomId = ;
            session.SendPackage(rsp);


            //TODO 收到登录请求，判断account是否有匹配的数据，如果有则关联，没有则创建
        }
    }
}
