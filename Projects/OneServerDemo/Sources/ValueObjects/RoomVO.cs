using System.Collections.Generic;

namespace OneServer
{
    class RoomVO
    {
        /// <summary>
        /// 角色列表
        /// </summary>
        public List<RoleVO> roles = new List<RoleVO>();

        /// <summary>
        /// 聊天内容列表
        /// </summary>
        public List<ChatVO> chats = new List<ChatVO>();
    }
}
