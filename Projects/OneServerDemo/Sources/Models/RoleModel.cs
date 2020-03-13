using One;
using One.WebSocket;
using System.Collections.Generic;

namespace OneServer
{
    class RoleModel : BaseModel
    {
        static int _roleIdIndex = 0;

        Dictionary<string, RoleVO> _dic = new Dictionary<string, RoleVO>();

        public RoleVO CreateRole(string account, string nickname)
        {
            RoleVO vo = new RoleVO();

            vo.account = account;
            vo.nickname = nickname;
            vo.id = ++_roleIdIndex;

            _dic[account] = vo;
            return vo;
        }        

        public RoleVO FindRole(string account)
        {
            if (_dic.ContainsKey(account))
            {
                return _dic[account];
            }
            return null;
        }

        public RoleVO FindRole(Session session)
        {
            foreach(var role in _dic.Values)
            {
                if(role.session == session)
                {
                    return role;
                }
            }
            return null;
        }

        protected override void Reset()
        {
            
        }
    }
}
