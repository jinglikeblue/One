using One;
using One.WebSocket;
using System.Threading;

namespace OneServer
{
    class ReqLoginReceiver : BaseServerReceiver<ReqLogin>
    {
        protected override void OnReceive(Session session, ReqLogin pbObj)
        {
            OneLog.D("[T:{0} S:{1}],登录请求：{2}", Thread.CurrentThread.ManagedThreadId, session.id, pbObj.Nickname);            

            var role = Global.Ins.roles.FindRole(pbObj.Account);
            if(null == role)
            {
                //创建一个角色
                role = Global.Ins.roles.CreateRole(pbObj.Account, pbObj.Nickname);                
            }
            else
            {                
                role.session?.Close();                
            }

            role.session = session;

            Global.Ins.room.AddRole(role);

            RspLogin rsp = new RspLogin();
            rsp.Id = role.id;
            //rsp.RoomId = ;
            session.SendPackage(rsp);

            PushRole pushRole = new PushRole();
            pushRole.Id = role.id;
            pushRole.Nickname = role.nickname;

            session.sessionManager.PushPackage(pushRole);


            //TODO 收到登录请求，判断account是否有匹配的数据，如果有则关联，没有则创建
        }
    }
}
