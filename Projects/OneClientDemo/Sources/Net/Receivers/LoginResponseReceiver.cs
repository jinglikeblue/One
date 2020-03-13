using One;
using One.WebSocket;
using OneServer;

namespace OneClient
{
    class LoginResponseReceiver : BaseClientReceiver<LoginResponseVO>
    {
        protected override void OnReceive(Client client, LoginResponseVO obj)
        {
            OneLog.D("登录返回：{0}", obj.id);
        }
    }
}
