using One.WebSocket;

namespace OneServer
{
    class RoleVO
    {
        /// <summary>
        /// 服务器生成的角色ID
        /// </summary>
        public int id;

        /// <summary>
        /// 账号ID
        /// </summary>
        public string account;

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname;

        /// <summary>
        /// 关联的会话
        /// </summary>
        public Session session;
    }
}
