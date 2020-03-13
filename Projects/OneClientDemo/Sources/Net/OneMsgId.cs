//命名空间和proto文件一致
namespace One
{
    /// <summary>
    /// push:推送消息(S2C) req:请求消息(C2S) resp:回复消息(S2C)
    /// </summary>
    class OneMsgId //proto文件名 + "MsgId"
    {

        //请求登录
        public const int ReqLogin = 1;

        //登录回复
        public const int RspLogin = 2;

        //推送角色状态
        public const int PushRole = 3;

        //发送消息
        public const int ReqMsg = 4;

        //推送消息
        public const int PushMsg = 5;

    }
}
