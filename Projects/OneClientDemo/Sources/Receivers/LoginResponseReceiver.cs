using Newtonsoft.Json;
using One;
using Share;

namespace OneClient
{
    class LoginResponseReceiver : BaseMessageReceiver<LoginResponseVO>
    {
        protected override void OnReceive(long requestId, LoginResponseVO data)
        {
            Log.I("收到协议:" + JsonConvert.SerializeObject(data));
        }
    }
}
